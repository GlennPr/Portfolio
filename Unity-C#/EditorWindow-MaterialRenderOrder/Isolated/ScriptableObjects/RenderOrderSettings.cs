using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EditorTools
{
	public class RenderOrderSettings : ScriptableObject
	{
		[HideInInspector] public Material[] Materials;
		[HideInInspector] public Shader[] Shaders;
		[HideInInspector] public int[] ShadersRenderQueue;

		[HideInInspector] public string[] GroupNames;
		[HideInInspector] public Color[] GroupColors;

		[HideInInspector] public Slot[] Slots;

		public void CheckValidity()
		{
			if (Materials == null || Materials.Length == 0) return;

			for (int i = Materials.Length - 1; i > -1; i--)
			{
				if (Materials[i] == null)
				{
					for (int j = 0; j < Slots.Length; j++)
					{
						var slot = Slots[j];
						if (slot.MaterialIndex == i)
						{
							RemoveSlot(slot);
							break;
						}
					}
				}
			}


			if (Shaders == null || Shaders.Length == 0) return;

			for (int i = Shaders.Length - 1; i > -1; i--)
			{
				if (Shaders[i] == null)
				{
					for (int j = 0; j < Slots.Length; j++)
					{
						var slot = Slots[j];
						if (slot.ShaderIndex == i)
						{
							RemoveSlot(slot);
							break;
						}
					}
				}
			}
		}

		public void RemoveSlot(Slot slot)
		{
			var slots = Slots.ToList();
			if (slots.Contains(slot))
			{
				slots.Remove(slot);
				Slots = slots.ToArray();

				if (slot.IsMaterialSlot())
				{
					var materials = Materials.ToList();
					materials.RemoveAt(slot.MaterialIndex);

					Materials = materials.ToArray();

					MaterialSlotGotRemoved(slot.MaterialIndex);
				}
				else
				{
					var shaders = Shaders.ToList();
					shaders.RemoveAt(slot.ShaderIndex);
					Shaders = shaders.ToArray();

					var shadersRenderQueue = ShadersRenderQueue.ToList();
					shadersRenderQueue.RemoveAt(slot.ShaderIndex);
					ShadersRenderQueue = shadersRenderQueue.ToArray();

					ShaderSlotGotRemoved(slot.ShaderIndex);
				}
			}
		}

		private void MaterialSlotGotRemoved(int index)
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				var slot = Slots[i];
				if (slot.IsMaterialSlot())
				{
					if (slot.MaterialIndex > index)
					{
						slot.MaterialIndex--;
					}
				}
			}
		}

		private void ShaderSlotGotRemoved(int index)
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				var slot = Slots[i];
				if (slot.IsMaterialSlot() == false)
				{
					if (slot.ShaderIndex > index)
					{
						slot.ShaderIndex--;
					}
				}
			}
		}

		public void RemoveGroup(int index)
		{
			var names = GroupNames.ToList();
			names.RemoveAt(index);
			GroupNames = names.ToArray();

			var colors = GroupColors.ToList();
			colors.RemoveAt(index);
			GroupColors = colors.ToArray();

			for (int i = 0; i < Slots.Length; i++)
			{
				var slot = Slots[i];

				if (slot.GroupIndex >= index)
				{
					slot.GroupIndex--;
				}
			}
		}

		public int GetQueueOfSlot(int slotIndex)
		{
			return GetQueueOfSlot(Slots[slotIndex]);
		}

		public int GetQueueOfSlot(Slot slot)
		{
			if (slot.IsMaterialSlot())
				return Materials[slot.MaterialIndex].renderQueue;
			else
				return ShadersRenderQueue[slot.ShaderIndex];
		}

		[System.Serializable]
		public class Slot {
			public int MaterialIndex = -1;
			public int ShaderIndex = -1;
			public int GroupIndex = -1;

			public bool IsMaterialSlot()
			{
				return MaterialIndex >= 0;
			}
		}
	}
}
