using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialBehaviour : MonoBehaviour
{
    public const float positiveInteractionResult = 2f;
    public const float neutralInteractionResult = 0.5f;
    public const float negativeInteractionResult = -1f;

    public enum SocialInteractions {  GOSSIP, CHAT , CRITICIZE, BOTHER, REPORT };
    public enum SocialState { FREE, ON_ROUTE, ON_QUEUE, BLOCKED, END};
    public float highPersonalityMultiplier = 1f;
    public float mediumPersonalityMultiplier = 0.75f;
    public float lowPersonalityMultiplier = 0.5f;
    public int AvaibleInteractions = 0;
    public SocialState socialState;

    private int AddedInteractionsOnNextDay = 0; //by default was 3, but is disabled
    private int InteractionsNeededToGoShop = 8;
   
    //Dayly variables//
    public List<WorldState.NPCName> NpcInteracted;
    private WorldState.NPCName npcToInteract;
    
    private void OnEnable()
    {
        if(AvaibleInteractions != 0)
        {
            socialState = SocialState.FREE;
            NpcInteracted = new List<WorldState.NPCName>();
        }
        else
        {
            socialState = SocialState.END;
        }
    }

    public float GetInteractionModifiers(Personality.PersonalityType targetPersonalityType) //this npc will interact with other and get a value
    {
        
        //new personalities must be added
       
        //all internal switch are equal, just 0.5 , 0.75 , 1 multiplier depending on personality
        switch(gameObject.GetComponent<Npc>().GetPersonality())
        {
            case Personality.PersonalityType.BLAB:
            {
                switch(targetPersonalityType)
                {
                    case Personality.PersonalityType.BLAB:
                    case Personality.PersonalityType.RUDE:
                    {
                        return highPersonalityMultiplier; //high affinity
                    }
                    case Personality.PersonalityType.LONELY:
                    case Personality.PersonalityType.FRIENDLY:
                    {
                        return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.FRIENDLY:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.SOCIABLE:
                    case Personality.PersonalityType.FRIENDLY:
                    case Personality.PersonalityType.LONELY:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    case Personality.PersonalityType.RUDE:
                    case Personality.PersonalityType.SELFISH:
                        {
                            return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.LONELY:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.SOCIABLE:
                    case Personality.PersonalityType.FRIENDLY:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    case Personality.PersonalityType.SELFISH:
                    case Personality.PersonalityType.RUDE:
                    case Personality.PersonalityType.PERFECTIONIST:
                    case Personality.PersonalityType.BLAB:
                        {
                            return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.PERFECTIONIST:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.SAVER:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.RUDE:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.BLAB:
                    case Personality.PersonalityType.SELFISH:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    case Personality.PersonalityType.SOCIABLE:
                    case Personality.PersonalityType.FRIENDLY:
                    case Personality.PersonalityType.LONELY:
                        {
                            return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.SAVER:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.SAVER:
                    case Personality.PersonalityType.SOCIABLE:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    case Personality.PersonalityType.SELFISH:
                        {
                            return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.SELFISH:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.SELFISH:
                    case Personality.PersonalityType.PERFECTIONIST:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    case Personality.PersonalityType.SAVER:
                    case Personality.PersonalityType.FRIENDLY:
                        {
                            return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.SOCIABLE:
            {
                switch (targetPersonalityType)
                {
                    case Personality.PersonalityType.SOCIABLE:
                    case Personality.PersonalityType.FRIENDLY:
                        {
                            return highPersonalityMultiplier; //high affinity
                        }
                    case Personality.PersonalityType.RUDE:
                        {
                            return lowPersonalityMultiplier; //low affinity
                        }
                    default:
                        return mediumPersonalityMultiplier; //medium affinity
                }
            }
            case Personality.PersonalityType.OTHER:
            {
                Debug.LogWarning("Personality not setted");
                return mediumPersonalityMultiplier; //medium affinity
            }
            default:
            {
                Debug.LogError("Personality out of list");
                return mediumPersonalityMultiplier;
            }
        }
    }

    private bool GetIfInteracted(WorldState.NPCName npcName)
    {
        for (int i = 0; i < NpcInteracted.Count; i++)
        {
            if(NpcInteracted[i] == npcName)
            {
                return true;
            }
        }
        return false;
    }

    private float GetInteractionValue()
    {
        return 0;
    }
    
    private void GetBetterInteraction()
    {

    }

    private void GetTargetToInteract ()
    {

    }

    private void GetIfTargetNear()
    {

    }

    private void GetIfValidTargetToInteractInScene()
    {

    }

    public void NextDay()
    {
        //AvaibleInteractions += AddMoreInteractions(); disabled, not finished
    }

    public void CheckInPosition()
    {
        switch(socialState)
        {
            case SocialState.ON_ROUTE:
            {
                //change state and interact
                break;
            }
        }
    }

    public int AddMoreInteractions()
    {
        return AddedInteractionsOnNextDay;
    }

    public int GetInteracionsNeededToGoShop()
    {
        return InteractionsNeededToGoShop;
    }

    public int DayOne()
    {
        return 0;
        //return Random.Range(0, InteractionsNeededToGoShop + AddedInteractionsOnNextDay); // the social behavieur is not finished
    }

    public void EnteredInQueue()
    {
        if (socialState != SocialState.END)
        {
            socialState = SocialState.ON_QUEUE;
            //interact on enter in queue
        }
    }

    //GOSSIP, CHAT , CRITICIZE, BOTHER, REPORT };

    public void Gossip()
    {

    }

    public void Chat()
    {

    }

    public void Criticize()
    {

    }

    public void Bother()
    {

    }

    public void Report()
    {

    }

}
