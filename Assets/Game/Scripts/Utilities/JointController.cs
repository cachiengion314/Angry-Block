using UnityEngine;

public class JointController : MonoBehaviour
{
  [SerializeField] RectTransform rootEndEffector;
  [SerializeField] RectTransform secondJoint;
  [SerializeField] RectTransform secondEndEffector;
  float secondJointLength;
  public float rotateSpeed;

  private void Start()
  {
    secondJointLength = (Camera.main.WorldToScreenPoint(secondEndEffector.transform.position)
                            - Camera.main.WorldToScreenPoint(secondJoint.transform.position)).magnitude;
  }

  private void Update()
  {
    if (Input.GetButton("Fire1"))
    {
      Vector3 mousePos = Input.mousePosition;

      RotateToTarget(mousePos, secondJoint, transform);

      var rootEffToMouseDistance = FindDistanceToTarget(mousePos, rootEndEffector);
      if (rootEffToMouseDistance > secondJointLength)
      {
        RotateToTarget(mousePos, transform);
      }
      else
        transform.Rotate(0, 0, -1 * Time.deltaTime * rotateSpeed);
    }
  }

  public float FindDistanceToTarget(Vector3 targetPos, Transform endEffector)
  {
    var endFactorScrPos = Camera.main.WorldToScreenPoint(endEffector.transform.position);
    endFactorScrPos.z = 0;
    return (targetPos - endFactorScrPos).magnitude;
  }

  public float FindAngleToTarget(Vector3 targetPos, Transform joint, Transform parent = null)
  {
    var jointPos = Camera.main.WorldToScreenPoint(joint.transform.position);
    jointPos.z = 0;
    var secondToMouseDir = (targetPos - jointPos).normalized;
    if (parent)
      return Vector3.Angle(secondToMouseDir, parent.TransformDirection(Vector3.right));
    return Vector3.Angle(secondToMouseDir, Vector3.right);
  }

  public void RotateToTarget(Vector3 targetPos, Transform currentJoint, Transform parentJoint = null)
  {
    var toTargetAngle = FindAngleToTarget(targetPos, currentJoint);
    if (parentJoint)
      toTargetAngle = FindAngleToTarget(targetPos, currentJoint, parentJoint);

    if (currentJoint.localRotation.eulerAngles.z < toTargetAngle)
      currentJoint.Rotate(0, 0, 1 * Time.deltaTime * rotateSpeed);
    else
    {
      if (currentJoint.localRotation.eulerAngles.z > 5)
        currentJoint.Rotate(0, 0, -1 * Time.deltaTime * rotateSpeed);
    }
  }
}
