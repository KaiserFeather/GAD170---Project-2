using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the outcome of a dance off between 2 dancers, determines the strength of the victory form -1 to 1
/// 
/// TODO:
///     Handle GameEvents.OnFightRequested, resolve based on stats and respond with GameEvents.FightCompleted
///         This will require a winner and defeated in the fight to be determined.
///         This may be where characters are set as selected when they are in a dance off and when they leave the dance off
///         This may also be where you use the BattleLog to output the status of fights
///     This may also be where characters suffer mojo (hp) loss when they are defeated
/// </summary>
public class FightManager : MonoBehaviour
{
    public Color drawCol = Color.gray;

    public float fightAnimTime = 2;

    private void OnEnable()
    {
        GameEvents.OnFightRequested += Fight;
    }

    private void OnDisable()
    {
        GameEvents.OnFightRequested -= Fight;
    }

    public void Fight(FightEventData data)
    {
        StartCoroutine(Attack(data.lhs, data.rhs));
    }

    IEnumerator Attack(Character lhs, Character rhs)
    {
        lhs.isSelected = true;
        rhs.isSelected = true;
        lhs.GetComponent<AnimationController>().Dance();
        rhs.GetComponent<AnimationController>().Dance();

        yield return new WaitForSeconds(fightAnimTime);

        float outcome = Random.Range(-1.0f, 1.0f); //creates a random number to determine winners

        float lhsformula = outcome * lhs.rhythm + lhs.luck / lhs.style; //formula for determining win chances
        lhsformula = Mathf.Clamp(lhsformula, -1.0f, 1.0f); //clamps the number in case of excesses

        float rhsformula = outcome * rhs.rhythm + rhs.luck / rhs.style;
        rhsformula = Mathf.Clamp(rhsformula, -1.0f, 1.0f);

        Character winner, defeated;
        if (lhsformula > rhsformula)
        {
            outcome = lhsformula;
            winner = lhs;
            defeated = rhs;
        }
        else
        {
            outcome = rhsformula;
            winner = rhs;
            defeated = lhs;
        }
        //discovers who is the winner and loser

        BattleLog.Log(new DefaultLogMessage("The winner is" + winner.charName.GetFullCharacterName(), winner.myTeam.teamColor));
        BattleLog.Log(new DefaultLogMessage("The loser is" + defeated.charName.GetFullCharacterName(), defeated.myTeam.teamColor));
        //sends the data to the battlelog so it can be displayed in text to the player
        //Debug.LogWarning("Attack called, needs to use character stats to determine winner with win strength from 1 to -1. This can most likely be ported from previous brief work.");
        //Debug.LogWarning("Attack called, may want to use the BattleLog to report the dancers and the outcome of their dance off.");

        var results = new FightResultData(winner, defeated, outcome);
        //sends the results of the fight to be stored

        lhs.isSelected = false;
        rhs.isSelected = false;
        GameEvents.FightCompleted(results);

        yield return null;
    }
}
