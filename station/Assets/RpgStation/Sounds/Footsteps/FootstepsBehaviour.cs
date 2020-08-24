
using UnityEngine;


namespace Station
{
    public class FootstepsBehaviour :  MonoBehaviour
{
	#region [[ FIELDS ]]
	
	[SerializeField] private FootSoundTemplate _soundTemplates;
	private RaycastHit _hit;
	private Renderer _walkedSurface;
	private TerrainData _terrainData;
	private SplatPrototype[] _splatPrototypes = new SplatPrototype[0];
	private SplatPrototype[] MSplatPrototypes
	{
		get
		{
			if (_splatPrototypes.Length == 0)
				CacheTerrain ();
			return _splatPrototypes;
		}
	}

	private SurfaceSceneReference _sceneReference;
	
	private Terrain _terrain;
	#endregion

	public void Setup(FootSoundTemplate template)
	{
		_soundTemplates = template;
	}

	public virtual void PlayFootstep()
	{
		if (_sceneReference == null)
		{
			_sceneReference = FindObjectOfType<SurfaceSceneReference>();
		}

		if (_sceneReference == null || _soundTemplates == null)
		{
			return;
		}

		// loop through the surfaces
		string surfaceKey = _sceneReference.ResolveTextureAsSurface(TextureUnderFeat);
		var surface = _soundTemplates.ResolveSurface(surfaceKey);
		PlaySound(surface);
	}


	private Texture TextureUnderFeat
	{
		get
		{
			Ray ray = new Ray(transform.position + Vector3.up * 0.2f, Vector3.down);

			if (!Physics.Raycast (ray, out _hit)) 
			{
				return null;
			}

		
			_terrain = _hit.collider.GetComponent<Terrain>();


			if(_terrain)
			{
				_terrainData = _terrain.terrainData;
				_splatPrototypes = _terrain.terrainData.splatPrototypes;
				return MSplatPrototypes[GetMainTexture(transform.position)].texture;
			}

			_walkedSurface = _hit.collider.GetComponent<Renderer> ();
			
			return _walkedSurface ? _walkedSurface.material.mainTexture : null;
		}
	}

	protected virtual void PlaySound(SurfaceEntry surface )
	{
		if(surface.Sounds == null)
			return;
		
		SoundSystem.PlayFootStep(surface.Sounds.name, transform);
	}

	#region from terrain
	public int GetMainTexture(Vector3 worldPos) 
	{
		float[] mix = GetTextureMix(worldPos);
		float maxMix = 0;
		int maxIndex = 0;

		for (int n=0; n<mix.Length; ++n){

			if (mix[n] > maxMix){
				maxIndex = n;
				maxMix = mix[n];
			}
		}

		return maxIndex;
	}

	public float[] GetTextureMix(Vector3 worldPos) {


		Vector3 terrainPos = _terrain.transform.position;
		int mapX = (int)(((worldPos.x - terrainPos.x) / _terrainData.size.x) * _terrainData.alphamapWidth);
		int mapZ = (int)(((worldPos.z - terrainPos.z) / _terrainData.size.z) * _terrainData.alphamapHeight);
		if (mapX >= _terrainData.alphamapWidth)
			mapX = _terrainData.alphamapWidth-1;
		if (mapZ >= _terrainData.alphamapHeight)
			mapZ  = _terrainData.alphamapHeight-1;
		
		float[,,] splatmapData = _terrainData.GetAlphamaps(mapX,mapZ,1,1);

		float[] cellMix = new float[splatmapData.GetUpperBound(2)+1];
		for (int n=0; n<cellMix.Length; ++n)
		{
			cellMix[n] = splatmapData[0,0,n];    
		}
		return cellMix;        
	}

	void OnEnable()
	{
		CacheTerrain();
	}

	void CacheTerrain()
	{
		if(Terrain.activeTerrain)
		{
			_terrain = Terrain.activeTerrain;
			_terrainData = _terrain.terrainData;
			_splatPrototypes = _terrain.terrainData.splatPrototypes;
		}
	}
	#endregion


}
}