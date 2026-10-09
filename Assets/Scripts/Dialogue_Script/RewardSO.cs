using UnityEngine;

[CreateAssetMenu(fileName = "RewardSO", menuName = "Dialogue/Reward")]
public class RewardSO : ScriptableObject
{
    [TextArea(2, 5)]
    public string rewardText;
}

