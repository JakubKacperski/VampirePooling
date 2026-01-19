using UnityEngine;

public class PausableWait : CustomYieldInstruction
{
    private float _targetTime;
    private float _elapsedTime;

    public override bool keepWaiting
    {
        get
        {
            if (!GameManager.Instance.IsPaused)
            {
                _elapsedTime += Time.deltaTime;
            }

            return _elapsedTime < _targetTime;
        }
    }

    public PausableWait(float seconds)
    {
        _targetTime = seconds;
    }
}