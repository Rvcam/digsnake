using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtil
{
    public class Tagger : MonoBehaviour
    {
        private HashSet<string> tags;
        public string[] editorTags;
        public void addCustomTag(string newTag)
        {
            tags.Add(newTag);
        }

        private void Awake()
        {
            tags = new HashSet<string>();
            foreach (string tag in editorTags)
            {
                tags.Add(tag);
            }
        }

        public bool containsCustomTag(string query)
        {
            return tags.Contains(query);
        }
    }
}
