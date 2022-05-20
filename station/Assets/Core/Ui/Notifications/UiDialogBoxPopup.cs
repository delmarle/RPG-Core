using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Station
{
	public class UiDialogBoxPopup : UiNotificationElement
	{
		
		//TODO on scene quit clear queue and hide if visible
		private DialogBoxData _data;
		private Queue<DialogBoxData> _queue = new Queue<DialogBoxData>();

		public UnityEngine.UI.Button Button1;
		public UnityEngine.UI.Button Button2;
		public UnityEngine.UI.Button Button3;

		public TMP_InputField Input;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Text;
		private bool _wasDialogShown;

		protected override void Awake()
		{
			base.Awake();
			_queue = new Queue<DialogBoxData>();

			// Disable itself
			if (!_wasDialogShown)
			{
				Hide();
			}
		}
		
		public override void ReceiveNotification(Dictionary<string, object> data)
		{
			if (data.ContainsKey(UiConstants.DIALOGUE_DATA))
			{
				DialogBoxData dbd = (DialogBoxData)data[UiConstants.DIALOGUE_DATA];
				if (dbd != null)
				{
					ShowDialog(dbd);
				}
			}
		}

		private void AfterHandlingClick()
		{
			if (_queue.Count > 0)
			{
				ShowDialog(_queue.Dequeue());
				return;
			}

			CloseDialog();
		}

		public void OnLeftClick()
		{
			_data.LeftButtonAction?.Invoke();

			AfterHandlingClick();
		}

		public void OnMiddleClick()
		{
			_data.MiddleButtonAction?.Invoke();

			AfterHandlingClick();
		}

		public void OnRightClick()
		{
			_data.RightButtonAction?.Invoke();

			if (_data.ShowInput)
				_data.InputAction.Invoke(Input.text);

			AfterHandlingClick();
		}

		public void ShowDialog(DialogBoxData data)
		{
			_wasDialogShown = true;
			ResetAll();

			_data = data;

			if ((data == null) || (data.Message == null))
				return;

			// Show the dialog box
			Show();
			Title.text = data.Title;
			Text.text = data.Message;

			var buttonCount = 0;

			if (!string.IsNullOrEmpty(data.LeftButtonText))
			{
				// Setup Left button
				Button1.gameObject.SetActive(true);
				Button1.GetComponentInChildren<Text>().text = data.LeftButtonText;
				buttonCount++;
			}

			if (!string.IsNullOrEmpty(data.MiddleButtonText))
			{
				// Setup Middle button
				Button2.gameObject.SetActive(true);
				Button2.GetComponentInChildren<TextMeshProUGUI>().text = data.MiddleButtonText;
				buttonCount++;
			}

			if (!string.IsNullOrEmpty(data.RightButtonText))
			{
				// Setup Right button
				Button3.gameObject.SetActive(true);
				Button3.GetComponentInChildren<TextMeshProUGUI>().text = data.RightButtonText;
			}
			else if (buttonCount == 0)
			{
				// Add a default "Close" button if there are no other buttons in the dialog
				Button3.gameObject.SetActive(true);
				Button3.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
			}

			Input.gameObject.SetActive(data.ShowInput);
			Input.text = data.DefaultInputText;


			transform.SetAsLastSibling();
		}

		private void OnDialogEvent(DialogBoxData data)
		{
			if (IsVisible)
			{
				// We're already showing something
				// Display later by adding to queue
				_queue.Enqueue(data);
				return;
			}

			ShowDialog(data);
		}

		private void ResetAll()
		{
			Button1.gameObject.SetActive(false);
			Button2.gameObject.SetActive(false);
			Button3.gameObject.SetActive(false);
			Input.gameObject.SetActive(false);
		}

		private void CloseDialog()
		{
			Hide();
		}

		public static void ShowError(string error)
		{
			// Fire an event to display a dialog box.
			// We're not opening it directly, in case there's a custom 
			// dialog box event handler
			// DialogBoxData.CreateError(error);
		}
	}

public class DialogBoxData
{
	private const string TITLE_TEXT = "Information";
	public string DefaultInputText = "";
	public Action<string> InputAction;
	public Action LeftButtonAction;

	public string LeftButtonText;
	public Action MiddleButtonAction;

	public string MiddleButtonText;
	public Action RightButtonAction;

	public string RightButtonText = "Close";

	public bool ShowInput;

	public DialogBoxData(string message, string title = TITLE_TEXT)
	{
		Title = title;
		Message = message;
	}

	public string Title{ get; private set; }
	public string Message { get; private set; }

	public static DialogBoxData CreateInfo(string message, string title = TITLE_TEXT)
	{
		return new DialogBoxData(message, title);
	}

	public static DialogBoxData CreateTextInput(string message, Action<string> onComplete,
		string rightButtonText = "OK", string title = "")
	{
		var data = new DialogBoxData(message)
		{
			ShowInput = true,
			RightButtonText = rightButtonText,
			InputAction = onComplete,
			LeftButtonText = "Close",
			Title =  title
		};
		return data;
	}

	public static DialogBoxData CreateActionBox(string message,Action onClickLeft, Action onClickRight,
		string leftButtonText ,string rightButtonText = "Cancel", string title = TITLE_TEXT)
	{
		var data = new DialogBoxData(message)
		{
			ShowInput = false,
			RightButtonText = rightButtonText,
			RightButtonAction = onClickRight,
			LeftButtonText = leftButtonText,
			LeftButtonAction = onClickLeft,
			Title =  title
		};
		return data;
	}
    }
}

