using Modding;
using ProgramableController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramableController.Script {
    public class BlockTagAddon : MonoBehaviour {
        private MText _tag;

        public void Awake() {
            if (_tag == null) {
                _tag = GetComponent<BlockBehaviour>().AddText("class name", Const.KEY_BLOCK_ADDITION_TAG, "");
            }
        }

        public string[] GetTags() {
			if (string.IsNullOrEmpty(_tag.Value)) {
				return new string[0];
			}
			return _tag.Value.Split(',').Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e)).ToArray();
		}
    }
}
