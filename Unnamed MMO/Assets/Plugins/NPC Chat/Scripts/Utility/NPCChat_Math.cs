namespace TurnTheGameOn.NPCChat
{
	using UnityEngine;

	public static class NPCChat_Math
	{
		public static float LinearDistance (float _start, float _end, float _position) 
		{
			return Mathf.InverseLerp (_start, _end, _position);
		}
	}
}