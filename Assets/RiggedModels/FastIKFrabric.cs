using UnityEngine;
using UnityEditor;
using System;

public class FastIKFrabric : MonoBehaviour
{
    public int chainLength = 2; // number of bones in the chain

    public Transform target; // position where the end of the chain tries to reach
    public Transform pole; //controls the middle chain's direction

    public int iterations = 10; //controls the accuracy of the IK calculations
    public float delta = 0.001f; //small tolerance to decide when to stop adjusting

    [Range(0,1)] public float snapBackStrength = 1.0f; //how much the bones snap back to their original positions

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] currentBonePositions;
    protected Vector3[] relativeStartDirection;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;
    void Awake()
    {
        Init();
    }
    void Init()
    {
        bones = new Transform[chainLength + 1]; //stores each bone in the chain
        currentBonePositions = new Vector3[chainLength + 1]; //current position of each bone
        bonesLength = new float[chainLength];
        relativeStartDirection = new Vector3[chainLength + 1]; //saves the starting direction each bone points towards
        startRotationBone = new Quaternion[chainLength + 1]; // saves the starting rotation of each bone

        if (target == null)
        {
            target = new GameObject(gameObject.name + "Target").transform;
            target.position = transform.position;
        }
        startRotationTarget = target.rotation;
        completeLength = 0.0f; // initialize total length of the bone chain

        var currentTransform = transform;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = currentTransform;
            startRotationBone[i] = currentTransform.rotation;
            if (i == bones.Length - 1) //find leaf bone
            {
                relativeStartDirection[i] = target.position - currentTransform.position; // only the leaf bone points to the target
            }
            else // find all other bones
            {

                relativeStartDirection[i] = bones[i + 1].position - currentTransform.position; // every other bone points towards the bone infront of it
                bonesLength[i] = relativeStartDirection[i].magnitude;
                completeLength += bonesLength[i]; //find totoal length of chain
            }

            currentTransform = currentTransform.parent;
        }

        if (bones[0] == null)
        {
            throw new Exception("The chain value is longer than the ancestor chain");
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null) 
            return;

        if (bonesLength.Length != chainLength)
        {
            Init();
            Debug.LogWarning("Chain Length does not match the actual length of the chain and has reset the bone transform.");

        }

        //get positions
        for (int i = 0; i < bones.Length; i++)
        {
            currentBonePositions[i] = bones[i].position;
        }

        if ((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength) // if the target to root length is greater than the complete length
        {
            var direction = (target.position - currentBonePositions[0]).normalized; // draws a straight line between root bone and target positions

            for (int i = 1; i < currentBonePositions.Length; i++)
            {
                currentBonePositions[i] = currentBonePositions[i - 1] + direction * bonesLength[i - 1]; // puts the current bone position on the direction line
            }
        }
        else // when target to root length is less than the complete length
        {
            var currentRootRotation = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
            var currentRootRotationDifference = currentRootRotation * Quaternion.Inverse(startRotationRoot);

            for (int i = 0; i < currentBonePositions.Length - 1; i++) //Snapback Strength influence
            {
                currentBonePositions[i + 1] = Vector3.Lerp(
                    currentBonePositions[i + 1],
                    currentBonePositions[i] + currentRootRotationDifference * relativeStartDirection[i],
                    snapBackStrength
                    );
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                //backwards movement
                for (int i = currentBonePositions.Length - 1; i > 0; i--)
                {
                    if (i == currentBonePositions.Length - 1)
                    {
                        currentBonePositions[i] = target.position; //leaf bone position is always the same as the targets position 
                    }
                    else
                    {
                        currentBonePositions[i] = currentBonePositions[i + 1] + (currentBonePositions[i] - currentBonePositions[i + 1]).normalized * bonesLength[i];
                    }
                }

                //forward movement
                for (int i = 1; i < currentBonePositions.Length; i++)
                {
                    currentBonePositions[i] = currentBonePositions[i - 1] + (currentBonePositions[i] - currentBonePositions[i - 1]).normalized * bonesLength[i - 1];
                }



                if ((currentBonePositions[currentBonePositions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                    break;

            }
        }

        //pole position bias
        if (pole != null)
        {
            for (int i = 1; i < currentBonePositions.Length - 1; i++)
            {
                var plane = new Plane(
                    currentBonePositions[i + 1] - currentBonePositions[i - 1],
                    currentBonePositions[i -1]
                    ); // draws infinite plane facing the direction between the last and next bone positions
                var projectedPole = plane.ClosestPointOnPlane(pole.position); // pole position on plane
                var projectedBone = plane.ClosestPointOnPlane(currentBonePositions[i]); // bone position on plane
                var angle = Vector3.SignedAngle(projectedBone - currentBonePositions[i - 1], projectedPole - currentBonePositions[i - 1], plane.normal);

                currentBonePositions[i] = Quaternion.AngleAxis(angle, plane.normal) * (currentBonePositions[i] - currentBonePositions[i - 1]) + currentBonePositions[i - 1];
            }
        }

        //set positions
        for (int i = 0; i < currentBonePositions.Length; i++)
        {
            if (i == currentBonePositions.Length - 1)
            {
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            }
            else
            {
                bones[i].rotation = Quaternion.FromToRotation(relativeStartDirection[i], currentBonePositions[i + 1] - currentBonePositions[i]) * startRotationBone[i];
                bones[i].position = currentBonePositions[i];
            }
        }
    }

    void OnDrawGizmos()
    {
        var current = this.transform;
        for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(
                current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale)
                );
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
