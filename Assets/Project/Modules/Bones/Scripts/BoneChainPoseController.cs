using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneChainPoseController : MonoBehaviour
{
    [Header("CURL SETTINGS")]
    [SerializeField, Range(3, 6)] private int _bonesPerCurl = 3;
    [SerializeField, Range(0.0f, 1.0f)] private float _curlSpacing = 0.02f;
    [SerializeField] private bool _incrementBonesPerCurl = true;

    [SerializeField, Range(0.0f, 1.0f)] private float _curlDuration = 0.2f;
    [SerializeField] private Ease _curlEase = Ease.InOutSine;


    [Header("STRAIGHTEN SETTINGS")]
    [SerializeField, Range(0.0f, 1.0f)] private float _straightenDuration = 0.2f;
    [SerializeField] private Ease _straightenEase = Ease.InOutSine;


    public void StraightenBonesInstantly(BoneChain boneChain, Transform target)
    {
        Bone[] bones = boneChain.Bones;

        bones[0].LookAt(target);
        bones[0].SetLocalPosition(Vector3.zero);

        for (int i = 1; i < bones.Length; ++i)
        {
            bones[i].SetLocalRotation(Quaternion.identity);
            bones[i].SetLocalPosition(Vector3.zero);
        }
    }

    public void StraightenBones(BoneChain boneChain, Transform target)
    {
        Bone[] bones = boneChain.Bones;

        bones[0].LookAt(target, _straightenDuration, _straightenEase);
        bones[0].SetLocalPosition(Vector3.zero, _straightenDuration, _straightenEase);

        for (int i = 1; i < bones.Length; ++i)
        {
            bones[i].SetLocalRotation(Quaternion.identity, _straightenDuration, _straightenEase);
            bones[i].SetLocalPosition(Vector3.zero, _straightenDuration, _straightenEase);
        }
    }


    public void CurlBonesInstantly(BoneChain boneChain)
    {
        Bone[] bones = boneChain.Bones;

        Vector3 curlAxis = Vector3.right;
        for (int i = 0; i < bones.Length; ++i)
        {
            int curlLoop = i / _bonesPerCurl;
            int bonesPerCurl = _incrementBonesPerCurl ? (_bonesPerCurl + curlLoop) : _bonesPerCurl;

            Quaternion rotation = Quaternion.AngleAxis(360f / bonesPerCurl, curlAxis);

            bones[i].SetLocalRotation(rotation);
            bones[i].SetLocalPosition(curlAxis * _curlSpacing);
        }
    }

    public void CurlBones(BoneChain boneChain)
    {
        Bone[] bones = boneChain.Bones;

        Vector3 curlAxis = Vector3.right;
        for (int i = 0; i < bones.Length; ++i)
        {
            int curlLoop = i / _bonesPerCurl;
            int bonesPerCurl = _incrementBonesPerCurl ? (_bonesPerCurl + curlLoop) : _bonesPerCurl;

            Quaternion rotation = Quaternion.AngleAxis(360f / bonesPerCurl, curlAxis);

            bones[i].SetLocalRotation(rotation, _curlDuration, _curlEase);
            bones[i].SetLocalPosition(curlAxis * _curlSpacing, _curlDuration, _curlEase);
        }
    }
}
