using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(IConstraint))]
public class ConstraintAnimator : MonoBehaviour
{
    private IConstraint m_Constraint;

    // Start is called before the first frame update
    void Start()
    {
        this.m_Constraint = this.transform.GetComponent<IConstraint>();
    }

    public void AnimateWeightTo(float weight)
    {
        StartCoroutine(AnimateWeightToCoroutine(weight));
    }

    private IEnumerator AnimateWeightToCoroutine(float weight)
    {
        float start = this.m_Constraint.weight;
        float time = 0;
        while (time < 1)
        {
            this.m_Constraint.weight = Mathf.SmoothStep(start, weight, time);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
    }
}
