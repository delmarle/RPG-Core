using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_UpdateManager;

namespace Station
{
    //Requires Canvas Renderer. Used for UI
    public class FTextCache
    {
        private Dictionary<string, Mesh> _infoCache = new Dictionary<string, Mesh>();


        public void CacheInfo(string name, Mesh info)
        {
            _infoCache[name] = info;
        }

        public Mesh GetInfo(string name)
        {
            return _infoCache.Get(name);
        }
    }




    public class UiText : TextMeshProUGUI
    {
        #region properties

      //  private LocalizationManager _lm;
        private static Dictionary<string, FTextCache> _pools = new Dictionary<string, FTextCache>();

        [SerializeField] private string _localizedKey;

        private string _fTextCacheId;
        private FTextCache _fTextCache;

        public string FTextCacheId
        {
            set
            {
                _fTextCacheId = value;
                FTextCache textCache = _pools.Get(value);
                if (textCache == null)
                {
                    textCache = new FTextCache();
                    _pools[value] = textCache;
                    this._fTextCache = textCache;
                }
                else
                {
                    this._fTextCache = textCache;
                }
            } 
        }


        public string LocalizedKey
        {
            
            get { return _localizedKey; }
            set
            {
                if (_localizedKey != value)
                {
                    _localizedKey = value;
                    if (Application.isPlaying)
                    {
                        OnLocalizationChange("");
                    }
                }
            }
        }


        [SerializeField] private bool _isDynimcallyAssigned;
        private CanvasRenderer _canvasRenderer;

        public bool IsDynimcallyAssigned
        {
            get { return _isDynimcallyAssigned; }
            set
            {
                if (_isDynimcallyAssigned != value)
                {
                    _isDynimcallyAssigned = value;
                    if (Application.isPlaying)
                    {
                        OnLocalizationChange("");
                    }
                }
            }
        }

        #endregion

        #region methods

        protected override void Awake()
        {
            base.Awake();
       
            this._fTextCacheId = "";
            this._fTextCache = null;
            this._canvasRenderer = this.GetComponent<CanvasRenderer>();
            if (Application.isPlaying)
            {
                /*
                if (_lm == null)
                {
                    //_lm = FS.Lm;
                }

                if (_lm != null && _lm.OnLocalizationChange != null)
                    _lm.OnLocalizationChange.AddListener(OnLocalizationChange);
*/
                if (font == null)
                {
//                    font = TMP_FontAsset.defaultFontAsset;// Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                }

                AssignValue();
            }
        }

        void OnLocalizationChange(string s)
        {
        //    if (!string.IsNullOrEmpty(_localizedKey) && !IsDynimcallyAssigned && _lm != null)
          //      text = _lm.GetValue(_localizedKey);
        }

        private void AssignValue()
        {
        //    if (!string.IsNullOrEmpty(_localizedKey) && !IsDynimcallyAssigned && _lm != null)
          //      text = _lm.GetValue(_localizedKey);
        }

        protected override void OnDestroy()
        {
            if (_fTextCache != null && Application.isPlaying)
            {
                this.m_mesh = null;
            }

            base.OnDestroy();
            if (Application.isPlaying)
            {
              //  if (_lm != null && _lm.OnLocalizationChange != null)
                //    _lm.OnLocalizationChange.RemoveListener(OnLocalizationChange);
            }
        }
/*
        public void SetLocalization(LocalizationManager lm)
        {
            _lm = lm;
        }
*/

        protected override void GenerateTextMesh()
        {
            if (_fTextCache == null || this.text == null || !Application.isPlaying || this.m_textInfo == null || this.m_textInfo.characterCount == 0)
            {
                base.GenerateTextMesh();
            }
            else
            {
                Mesh info = _fTextCache.GetInfo(this.text);
                if (info == null)
                {
                    base.GenerateTextMesh();
                    _fTextCache.CacheInfo(this.text, Instantiate(this.mesh));
                }
                else
                {
                    if(_canvasRenderer)
                        _canvasRenderer.SetMesh(_fTextCache.GetInfo(this.text));
                }
            }

            

        }
        #endregion
    }
}