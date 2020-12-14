using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


public class csShurikenEffectChanger : MonoBehaviour
{
    [System.Obsolete]
    public void ShurikenParticleScaleChange(float _Value)
	{
		ParticleSystem[] ParticleSystems = GetComponentsInChildren<ParticleSystem>();

        transform.localScale *= _Value;

		foreach(ParticleSystem _ParticleSystem in ParticleSystems) {
			_ParticleSystem.startSpeed *= _Value;
			_ParticleSystem.startSize *= _Value;
			_ParticleSystem.gravityModifier *= _Value;
			SerializedObject _SerializedObject = new SerializedObject(_ParticleSystem);
			if (_SerializedObject.FindProperty("CollisionModule.particleRadius") != null) _SerializedObject.FindProperty("CollisionModule.particleRadius").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ShapeModule.radius") != null) _SerializedObject.FindProperty("ShapeModule.radius").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ShapeModule.boxX") != null) _SerializedObject.FindProperty("ShapeModule.boxX").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ShapeModule.boxY") != null) _SerializedObject.FindProperty("ShapeModule.boxY").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ShapeModule.boxZ") != null) _SerializedObject.FindProperty("ShapeModule.boxZ").floatValue *= _Value;
			if (_SerializedObject.FindProperty("VelocityModule.x.scalar") != null) _SerializedObject.FindProperty("VelocityModule.x.scalar").floatValue *= _Value;
			if (_SerializedObject.FindProperty("VelocityModule.y.scalar") != null) _SerializedObject.FindProperty("VelocityModule.y.scalar").floatValue *= _Value;
			if (_SerializedObject.FindProperty("VelocityModule.z.scalar") != null) _SerializedObject.FindProperty("VelocityModule.z.scalar").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ClampVelocityModule.x.scalar") != null) _SerializedObject.FindProperty("ClampVelocityModule.x.scalar").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ClampVelocityModule.y.scalar") != null) _SerializedObject.FindProperty("ClampVelocityModule.y.scalar").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ClampVelocityModule.z.scalar") != null) _SerializedObject.FindProperty("ClampVelocityModule.z.scalar").floatValue *= _Value;
			if (_SerializedObject.FindProperty("ClampVelocityModule.magnitude.scalar") != null) _SerializedObject.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= _Value;
			_SerializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
