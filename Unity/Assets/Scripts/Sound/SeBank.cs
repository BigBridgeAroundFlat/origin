using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sound
{
    public class SeBank : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _seList = new List<AudioClip>();

        public AudioClip GetAudioClip(string fileName)
        {
            var audioClip = _seList.FirstOrDefault(x => x.name == fileName);
            return audioClip;
        }
    }

}