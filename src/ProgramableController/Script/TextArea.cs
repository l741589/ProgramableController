namespace ProgramableController {
    using Modding;
    using Modding.Mapper;
	using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class TextArea : MCustom<string> {
		public TextArea(string displayName, string key, string defaultValue) : base(displayName, key, defaultValue) {
			
		}

		public override string DeSerializeValue(XData data) {
			XString xstring = data as XString;
			return xstring != null ? xstring.Value : "";
		}

		public override XData SerializeValue(string value) {
			return new XString(base.SerializationKey, value);
		}
	}

	public class TextAreaSelector : CustomSelector<string, TextArea> {

		private MeshRenderer boxRenderer;
		private bool active;
		private int windowId;
		private Rect uiRect;
		private GUIStyle textStyle;
		private Vector2 scroll;

		protected override void CreateInterface() {
			var box = Elements.MakeBox(new Vector3(0, 0, 0), new Vector2(1.8f, 0.4f), Materials.DarkElement);
			boxRenderer = box.GetComponent<MeshRenderer>();
			var button = Elements.AddButton(box.transform);
			button.Click += Button_Click;
			var text = Elements.MakeText(new Vector3(0, 0, -1), CustomMapperType.DisplayName);
			active = true;
			UpdateInterface();
		}

		protected override void UpdateInterface() {
			if (active) {
				boxRenderer.material = Materials.RedHighlight;
			} else {
				boxRenderer.material = Materials.DarkElement;
			}
		}

		private void Button_Click() {
			active = !active;
			UpdateInterface();
		}

		private void InitFont() {
			if (this.textStyle != null) {
				return;
			}
			lock (this) {
				if (this.textStyle != null) {
					return;
				}
				this.windowId = ModUtility.GetWindowId();
				this.uiRect = new Rect(Screen.width - 750, Screen.height - 800, 0f, 0f);
				var textStyle = new GUIStyle(GUI.skin.textArea);
				textStyle.wordWrap = false;
				try {
					GameObject gameObject = GameObject.Find("_PERSISTENT/Canvas/ConsoleView/ConsoleViewContainer/Content/Scroll View/Viewport/Content/LogText");
					Text text = (gameObject != null) ? gameObject.GetComponent<Text>() : null;
					textStyle.font = text.font;
				} catch (Exception ex) {
					Debug.LogWarning("Load console font failed: "+ex.Message);
				}
				this.textStyle = textStyle;

			}
		}

		public void OnGUI() {
			if (!active) {
				return;
			}
			InitFont();
			this.uiRect = GUILayout.Window(this.windowId, new Rect(this.uiRect.position, Vector2.zero), _ => {
				GUILayout.BeginVertical(new GUILayoutOption[0]);


				this.scroll = GUILayout.BeginScrollView(this.scroll, new GUILayoutOption[]
				{
					GUILayout.Width(700f),
					GUILayout.Height(600f)
				});
				var val = GUILayout.TextArea(CustomMapperType.Value, this.textStyle, new GUILayoutOption[]{
					GUILayout.ExpandWidth(true),
					GUILayout.ExpandHeight(true)
				});
				if (val != CustomMapperType.Value) {
					CustomMapperType.Value = val;
					OnEdit();
				}
				GUILayout.EndScrollView();
				/*
				if (this.savedScript != this.lastScript) {
					this.savedScript = this.lastScript;
					Exception e = this.SelectedCpu.CheckScript(this.savedScript);
					this.UpdateScriptStatus(e);
				}*/
				//GUILayout.Label(this.statusText, new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUI.DragWindow();
			}, CustomMapperType.DisplayName, new GUILayoutOption[0]);
		}
	}

}


