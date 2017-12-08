using UnityEngine;
using System.Collections;
using System;

namespace Engine
{
    public class ResourceManager : MonoBehaviour 
	{
		static Hashtable cache = new Hashtable();

		public static Sprite LoadSprite(string path)
		{
			if(!cache.ContainsKey(path))
			{
				Sprite sprite = Resources.Load<Sprite>(path);
				cache.Add(path,sprite);
			}
			return (Sprite)cache[path];
		}

		public static Sprite LoadSprite(string path, string spriteName)
		{
			Sprite[] sprites;
			if(!cache.ContainsKey(path))
			{
				sprites = Resources.LoadAll<Sprite>(path);
				cache.Add(path,sprites);
			}

			sprites = (Sprite[])cache[path];
			return Array.Find<Sprite>( sprites, (sprite) => sprite.name.Equals(spriteName));
		}

		public static Texture LoadTexture(string path)
		{
			if(!cache.ContainsKey(path))
			{
				Texture tex = Resources.Load<Texture>(path);
				cache.Add(path,tex);
			}
			return (Texture)cache[path];
		}

        public static string LoadTextAsset(string path)
        {
            TextAsset file = Resources.Load<TextAsset>(path);
            return file.text;
        }

		public static GameObject LoadAndInstantiate(string path)
		{
			if (!cache.ContainsKey(path))
			{
				GameObject go = Resources.Load<GameObject>(path);
				cache.Add(path, go);
			}

			GameObject instance = Instantiate<GameObject>((GameObject)cache[path]);
			return instance;
		}

        public static AudioClip LoadAudioClip(string path)
        {
            if (!cache.ContainsKey(path))
            {
                AudioClip audio = Resources.Load<AudioClip>(path);
                cache.Add(path, audio);
            }
            return (AudioClip)cache[path];
        }

        public static RuntimeAnimatorController LoadRuntimeAnimatorController(string path)
        {
            if (!cache.ContainsKey(path))
            {
                RuntimeAnimatorController audio = Resources.Load<RuntimeAnimatorController>(path);
                cache.Add(path, audio);
            }
            return (RuntimeAnimatorController)cache[path];
        }

        public static AnimationClip LoadAnimationClip(string path)
        {
            if (!cache.ContainsKey(path))
            {
                AnimationClip audio = Resources.Load<AnimationClip>(path);
                cache.Add(path, audio);
            }
            return (AnimationClip)cache[path];
        }
    }
}
