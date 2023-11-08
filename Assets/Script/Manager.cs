using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject enemyHealthB;
    [SerializeField] private GameObject plrHealthB;
    [SerializeField] private GameObject skillsPanel;
    [SerializeField] private GameObject stateDisplay;
    [SerializeField] private GameObject moveDisplay;

    [SerializeField] private int eneHp = 100, plrHp = 100;
    private string lastMove;
    private bool endGame = false;

    public enum Roster {Player, AI};
    public enum MonsterState {Weak, Hostile, Neutral }
    public MonsterState monsterState = MonsterState.Hostile;
    [SerializeField] Roster currentTurn = Roster.Player;

    private string[] hostileSkilltbl = { "Bite", "Slap", "Bite"};
    private string[] weakSkilltbl = { "Slap", "Drink Water","Dance"};


    private int FixHealth(int num)
    {
        if (num <= 0) { num = 0; endGame = true; }
        else if (num > 100) { num = 100; }
        return num;
    }
    private void ChangeHealth(int num)
    {
        if (num > 0)
        {
            if (currentTurn == Roster.Player) { plrHp += num; }
            else { eneHp += num; }
        }
        else
        {
            if (currentTurn == Roster.Player) { eneHp += num; }
            else { plrHp += num; }
        }
        eneHp = FixHealth(eneHp);
        enemyHealthB.GetComponentInChildren<Text>().text = eneHp + "/100";
        enemyHealthB.GetComponent<Scrollbar>().size = (float)eneHp / 100;

        plrHp = FixHealth(plrHp);
        plrHealthB.GetComponentInChildren<Text>().text = plrHp + "/100";
        plrHealthB.GetComponent<Scrollbar>().size = (float)plrHp / 100;
    }
    private void SwitchTurn()
    {
        if (!endGame)
        {
            if (currentTurn == Roster.Player)
            {
                currentTurn = Roster.AI;
                StartCoroutine(EnemyTurn());
            }
            else
            {
                for (int i = 0; i < skillsPanel.transform.childCount; i++)
                {
                    skillsPanel.transform.GetChild(i).GetComponent<Button>().interactable = true;
                }
                currentTurn = Roster.Player;
            }
        }
    }
    public void Move(string Skill)
    {
        if (Skill == "Bite")
        {
            ChangeHealth(-25);
        }
        else if (Skill == "Slap")
        {
            ChangeHealth(-15);
        }
        else if (Skill == "Drink Water")
        {
            ChangeHealth(5);
        }
        else if (Skill == "Dance")
        {
            ChangeHealth(10);
        }
        lastMove = Skill;

        if (currentTurn == Roster.Player && !endGame)
        {
            for (int i = 0; i < skillsPanel.transform.childCount; i++)
            {
                skillsPanel.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            SwitchTurn();
        }
    }

    private IEnumerator EnemyTurn()
    {
        if(eneHp < 50) { monsterState = MonsterState.Weak; }
        else { monsterState = MonsterState.Hostile;}

        if (lastMove == "Dance") { monsterState = MonsterState.Neutral; }

        string chosenSkill;

        switch (monsterState)
        {
            case MonsterState.Weak:
                stateDisplay.GetComponent<Text>().text = "State: Weak";
                yield return new WaitForSeconds(1);
                chosenSkill = weakSkilltbl[Random.Range(0, weakSkilltbl.Length)];
                break;
            case MonsterState.Hostile:
                stateDisplay.GetComponent<Text>().text = "State: Hostile";
                yield return new WaitForSeconds(1);
                chosenSkill = hostileSkilltbl[Random.Range(0, hostileSkilltbl.Length)];
                break;
            case MonsterState.Neutral:
                stateDisplay.GetComponent<Text>().text = "State: Dance Time!";
                yield return new WaitForSeconds(1);
                chosenSkill = "Dance";
                break;
            default:
                chosenSkill = hostileSkilltbl[Random.Range(0, hostileSkilltbl.Length)];
                break;
        }
        moveDisplay.SetActive(true);
        moveDisplay.GetComponent<Text>().text = "Enemy use " + chosenSkill;
        Move(chosenSkill);
        yield return new WaitForSeconds(1);
        moveDisplay.SetActive(false);
        SwitchTurn();
    }
}
