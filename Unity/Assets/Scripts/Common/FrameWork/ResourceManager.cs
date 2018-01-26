using System;
using System.Collections;
using UnityEngine;

namespace Common.FrameWork
{
	public class ResourceManager : MonoBehaviour 
	{
		static readonly Hashtable Cache = new Hashtable();

		public static Sprite LoadSprite(string path)
		{
			if(!Cache.ContainsKey(path))
			{
				Sprite sprite = Resources.Load<Sprite>(path);
				Cache.Add(path,sprite);
			}
			return (Sprite)Cache[path];
		}

		public static Sprite LoadSprite(string path, string spriteName)
		{
			Sprite[] sprites;
			if(!Cache.ContainsKey(path))
			{
				sprites = Resources.LoadAll<Sprite>(path);
				Cache.Add(path,sprites);
			}

			sprites = (Sprite[])Cache[path];
			return Array.Find( sprites, (sprite) => sprite.name.Equals(spriteName));
		}

		public static GameObject LoadAndInstantiate(string path)
		{
			if (!Cache.ContainsKey(path))
			{
				GameObject go = Resources.Load<GameObject>(path);
				Cache.Add(path, go);
			}

			var instance = Instantiate((GameObject)Cache[path]);
			return instance;
		}

        public static AudioClip LoadAudioClip(string path)
        {
            if (!Cache.ContainsKey(path))
            {
                AudioClip audio = Resources.Load<AudioClip>(path);
                Cache.Add(path, audio);
            }
            return (AudioClip)Cache[path];
        }

        public static AnimationClip LoadAnimationClip(string path)
        {
            if (!Cache.ContainsKey(path))
            {
                AnimationClip audio = Resources.Load<AnimationClip>(path);
                Cache.Add(path, audio);
            }
            return (AnimationClip)Cache[path];
        }

	    public static RuntimeAnimatorController LoadRuntimeAnimatorController(string path)
	    {
	        if (!Cache.ContainsKey(path))
	        {
	            RuntimeAnimatorController audio = Resources.Load<RuntimeAnimatorController>(path);
	            Cache.Add(path, audio);
	        }
	        return (RuntimeAnimatorController)Cache[path];
	    }
    }
}
