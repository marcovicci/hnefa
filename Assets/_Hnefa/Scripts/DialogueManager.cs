using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public string mCurrentPiece;
    public int[] mCurrentEASA = new int[4];
    public GameObject CharacterPortrait;
    public GameObject TextBox;
    public GameObject mDirector;
    public int ConversationCount;
    public int LineCount;
    public BasePiece mCurrentPieceScript;
    public string[] AllEmotions = new string[26]
    {"Aggressive","Alarm","Anger","Anxiety","Aware",
    "Contempt","Curiosity","Cynicism","Delight","Despair",
    "Disappointed","Disbelief","Disgust","Distracted","Dominance",
    "Envy","Fear","Guilty","Happy","Love",
    "Morbidity","Neutral","Nostalgic","Optimism","Outrage",
    "Pessimistic"
    };


    public string[] TricksterDialogue = new string[]   
    {
        // 59-61 trickster
        "A darkened form approaches\nBe it shadow, or a mask?\nTo visit is uncommon\nSo what shall it say, I ask?",
        "Words I see are stolen\nTo pass to beak from mouth\nThough only those of deep emotion\nShall we use this to sow doubt?",
        "A trick, perhaps\nA final ruse\nA fable for our end\nSpeak now, your feelings\nAnd I'll provide\nA message for you to send"
    };

    public string[] JarlDialogue = new string[]
    {
        // jarl
        // 2, what nuisance is a regular bird?
        "50 frosts passed, I would have expected raiders sooner.",
        "Fly to the outermost homes, have the… axe… warrior… have them move to offense.",
        "Do not fear being seen by the invaders, what nuisance is a regular bird? Besides, the cover of night will mask your form. Go now, I will expect you at dawn."
    };

    public string[] SageDialogue = new string[]
    {
        // 3-10, sage
        // 3, What a lovable fool.
        // 10, I have long seen our doom, framed in the ruins of this town
        "Even now, the boy has no time to address his people. I know of the raid, raven. I know of our resources, or lack there-of. No doubt, the Jarl hunches over his father's board, quietly gleeful that it may see proper use. What a lovable fool.",
        "Twelve people, not one of them worthy of his attention.",
        "Not that I intend to devalue your efforts, friend. I am simply frustrated by our commander's misguided obsession.",
        "While it is true that his father maintained a somewhat… eccentric interest in hnefatafl, there was more to his leadership, more to his legacy.",
        "And for all of that! Our history, culture, thoughts, feelings… OUR LIVES AND LEGACY! For all of this, the boy sees only trinkets on a rotting slab of oak.",
        "Though I am old, there is still much I wished to do before my passing. Leave me for now, I must consider what I wish to be my last rites.",
        "Everything considered, we are in a far better place than we could be. The Jarl has made some form of preparation, at the least. Though it may not be the most immediately relevant.",
        "In all transparency, raven, I have long seen our doom, framed in the ruins of this town. The shadow of this raid was long, we should have acted the moment we saw the first signs of dilapidation."
        
    };

    public string[] OutlawDialogue = new string[]
    {
        // 11-15, outlaw
        // 15, I appreciate the company
        "Leave, last thing I need is a bloody magpie picking over my haul.",
        "Ass knows you'll have plenty to feed on, a few days passed.",
        "Town's on the brink of catastrophe. Panicked people forget things, clothes, jewelry… respect.",
        "If you can't find yourself a trampled corpse by week's end, I'll let you eat my bad eye! Just keep your wicked tongue off any loose trinkets.",
        "Look, I appreciate the company, but I need to get moving. Now get."
    };

    public string[] OrphanDialogue = new string[]
    {
        // 16-19 orphan
        // 17, There's no point in hysterics just yet
        // 18, To be honest, I don't mind dying.
        "Back off, last time we met you ran off with my day's meal.",
        "There's no point in hysterics just yet, it's not as if I haven't had practice avoiding people. Now the stakes are just a little higher.",
        "To be honest, I don't mind dying. I don't do much other than scavenge, and the living around me aren't particularly interesting. If you're going to die, I've heard that doing it during a battle is a good plan.",
        "They don't want to help me for my sake. They want to do it so they don't have to think about me anymore. It's their comfort, not mine."
    };

    public string[] LoverDialogue = new string[]
    {
        // 20-22, lover
        // 20, I doubt he'd appreciate such a garish piercing.
        "Oh? The Jarl sending for a final service? I'm honoured, but if the screams from the mountains are what I assume, I may not reach him without an arrow in my breast. I doubt he'd appreciate such a garish piercing.",
        "In another life, perhaps you would lay with me. Tell me what it is you feel in a way I can fully understand. The way beasts do.",
        "Though, only now I realise that my knowledge of the animal may be more limited than I assumed. Imagine it, a whore with an ego."
    };

    public string[] HeroDialogue = new string[]
    {
        // 23-27, hero
        // 23, Aha! Noble messenger!
        // 27, What, then, is this sinking in my chest?
        "Aha! Noble messenger! What have you to relay?",
        "No… orders… I was under the impression that was your purpose? Relaying instructions from the Jarl?",
        "A riddle then? While I knew the Jarl was a little juvenile in his hobbies, I never imagined he would be so playful in a time of crisis.",
        "Apologies, I should have more faith in a man so well trained in defensive tactics.",
        "So that's it then. I have no reason to complain, I suppose. The fabled battle, that all great warriors aspire to, has come to meet me. What, then, is this sinking in my chest?"
    };

    public string[] MotherDialogue = new string[]
    {
        // 28-29, mother
        // 28, they should be here!
        "You! Where is my family? Yes, the father tends to wander, but with all the noise from the hills… they should be here!",
        "Look, I don't want to get hysterical here but have you heard the noises? Up on the mountain?"
    };

    public string[] ChildDialogue = new string[]   
    {
        // 30-35, child
        // 30, THE BIRDY IS HERE! THE BIRDY IS HERE!
        "THE BIRDY IS HERE! THE BIRDY IS HERE! Are you here for corpses? Scraps are gone, I eat 'em.",
        "Lots of excited people on the mountains. No one's come to visit for years! I hope they have toys. Artist only makes sad ones now.",
        "Maybe I should go greet them! I don't want them to think we're rude!",
        "Mother would get angry if I went to the mountains alone, though. I should wait.",
        "I hope Hero is alright. He was all hoppy and running this afternoon. Happy is good, but he was so happy, it was scary.",
        "I'm staying inside, to wait for the visitors. Can you check on my Mother? She starts spinning if she doesn't know where I am."
    };

    public string[] CowardDialogue = new string[]    
    {
        // 35-45 coward
        // 35 whore's beard
        // 38 leave behind the maggots that infest it
        // 39 I deserve some-place better
        // 45 I might have better luck with a cloaca
        "Whore's beard, well now there's no question. We're going to die.",
        "I surely wouldn't be seeing you otherwise.",
        "Everyone in the village has heard the horns. Some of the more hopeful were just holding out that they'd herald some eccentric merchants. No such luck. Tafl is doomed.",
        "Not I, of course. I'm off to the hills. To be blunt, I've been looking for a chance to slide out of this bloated corpse-town for years, and leave behind the maggots that infest it.",
        "I deserve some-place better…warmer…some place where the broads aren't so clingy.",
        "I like to run. It's comfortable. When you're moving you're ready.",
        "Fourth place I've been, Tafl is, you'd think I'd have better self-control. Hey, maybe we should swap nethers, you get a taste of the good life and I get a MUCH needed handicap.",
        "Wait… say again?",
        "A mimic… Naturally. Doubt an ip like the Jarl would have taught you to be so verbose. Tell you what, don't tell my burden or her grub any of what I said, eh? Or better yet… come a bit closer.",
        "Granted, with whores like that Lover around, I might have better luck with a cloaca! Why don't you check on her? Desperate times, poor thing's probably looking for a last lay, and trust me, she's not picky.",
    };

    public string[] ArtistDialogue = new string[]
    {
        // 46-58 artist
        // 46 Little poet!
        // 46 we're both busy composing our swan songs.
        // 48 We are no warriors
        // 49 I have come to terms with my body's death.
        // 51 those precious artifacts will surely be ravaged.
        "Little poet! I thought you'd visit! It seems we're both busy composing our swan songs.",
        "Excuse me, 'raven song'.",
        "We are no warriors, and if you were to flee your post by the Jarl, you would have done so by now.",
        "I have come to terms with my body's death. To be completely honest, it's a wonder Tafl has stood for this long without a raid.",
        "Even if our flesh should rot here, though, I have formulated a plan that will preserve our spirits for generations.",
        "The Jarl's board, the pieces, most of his clothes… They are my family's work. If he is captured, which he likely will be, those precious artifacts will surely be ravaged.",
        "My intent, then, is to produce a simple carving that encapsulates the soul of the village, and her inhabitants, and bury it. So that it may be found in years to come.",
        "How do you fare, little poet? I have been drafting my magnum opus. Care to compare notes?",
        "See, over the past few years, I've created representations of the remaining villagers of Tafl, to the best of my ability. To be frank, I'm not interested in doing something that's already been done, even if it was my own work.",
        "So, I've decided, I will create a new game. A hnefatafl that better represents us. Stronger abstractions, new, more thoughtful moves. A game that will allow our souls to echo for millenia.",
        "It's a big idea, but if we're going to die, I see no reason to go out being unambitious.",
        "I don't know if it's really possible to live without fearing death. At the very least, I'm certain that I would not be alive, nor living the way that I am, without that fear."
    };

    public string[] WitchDialogue = new string[]  
    {
        // 61-witch
        // 61 Always a blessing
        "Always a blessing, fylgja, what have you heard today?",
        "I take it the village has noticed the encroaching darkness. They will be on us by morning, if they aren't already picking over the bones of the outskirts.",
        "Not much to pick over, I'm afraid. These day even the Jarl hold little in the way of riches. The raiders won't know that until they've slaughtered us though.",
        "Mmmmm. Tafl is little more but carrion now, I am afraid.",
        "I can only pray that our sacred sites have been left untouched. Though faith and foolishness are not sisters, I should assume that they have been toppled, if only out of spite.",
        "Wasn't it my duty though? To protect them? I may have been cast out long ago, but only by the living. My word, I'd forgotten my purpose. How shameful.",
        "That will be my mission then. My death was already close. If anything, I have been reminded of my duty. The town failed to trust the guidance of our ancestors before. If there is any hope for redemption, it is in properly honoring them as we pass through their hands."
    };

    public string[] SeekerDialogue = new string[]  
    {
        // seeker
        // I KNEW IT!
        // I'm wise to your machinations!
        "I KNEW IT! The Jarl's wicked plan is in motion. Come to see if I've succumbed, eh? Well, I hate to disappoint, but I'm wise to your machinations!",
        "We've been sold out to a greater empire. Not for our piddly goods, but for fun. 'Til now, I thought you were a part of it, but I'm beginning to question whether you'd be capable of comprehending the plan, let alone take part in it."
    };   
        

    void Start()
    {
        mDirector = GameObject.Find("GameDirectorPrefab");
        mDirector.GetComponent<Director>().ContinueConversationScene(this);
    }

    public void StartConversation(BasePiece currentPiece)
    {
        TextBox.GetComponent<DisplayText>().ClearConversationHistory();
        CharacterPortrait.GetComponent<CharacterSpriteDisplay>().ShowCorrectEmotion("Neutral");
        // Copy in important values from current piece at start of dialogue
        mCurrentPiece = currentPiece.name;
        mCurrentPieceScript = currentPiece;
        mCurrentEASA = currentPiece.mCurrentEASA;
        LineCount = currentPiece.mDialogueState;

        // Start conversation
        TextBox.GetComponent<DisplayText>().ConversationStarter(mCurrentPiece);
        
    }

    public void PlayerTalking(string dialogue, int[] easa, string voice)
    {
        // Called when the player selects dialogue
        TextBox.GetComponent<DisplayText>().AddRavenSpeak(dialogue, voice);
    }

    public string FindDialogue()
    {   
        string[] thisArray = null;
        // Disgusting if statements, we're kludging this
        if (mCurrentPiece == "Seeker") thisArray = SeekerDialogue;
        if (mCurrentPiece == "Jarl") thisArray = JarlDialogue;
        if (mCurrentPiece == "Witch") thisArray = WitchDialogue;
        if (mCurrentPiece == "Trickster") thisArray = TricksterDialogue;
        if (mCurrentPiece == "Hero") thisArray = HeroDialogue;
        if (mCurrentPiece == "Orphan") thisArray = OrphanDialogue;
        if (mCurrentPiece == "Sage") thisArray = SageDialogue;
        if (mCurrentPiece == "Lover") thisArray = LoverDialogue;
        if (mCurrentPiece == "Mother") thisArray = MotherDialogue;
        if (mCurrentPiece == "Child") thisArray = ChildDialogue;
        if (mCurrentPiece == "Artist") thisArray = ArtistDialogue;
        if (mCurrentPiece == "Coward") thisArray = CowardDialogue;
        if (mCurrentPiece == "Outlaw") thisArray = OutlawDialogue;
        Debug.Log(LineCount);
        Debug.Log(thisArray.Length);
        // Make sure we're not out of bounds
        if (LineCount >= thisArray.Length)
        {
            LineCount = 0;
            Debug.Log("Resetting to zero.");
        }

        // Done finding dialogue, increment and update for next time
        LineCount += 1;
        mCurrentPieceScript.mDialogueState = LineCount;

        // Safety for something going terribly wrong
        if (thisArray == null)
        {
            return "Hmm.";
        }

        return thisArray[(LineCount-1)];

        
    }

    public void PlayerTalkingTest()
    {
        TextBox.GetComponent<DisplayText>().AddRavenSpeak("Caw.", "raven itself");
        StartCoroutine(ShortDelayForTesting());    
    }

    IEnumerator ShortDelayForTesting()
    {
    
      yield return new WaitForSeconds(1);
      string mDialogue = FindDialogue();
      CharacterTalking(mDialogue);
      UpdateEmotion();
    }

    public void CharacterTalking(string dialogue)
    {
        TextBox.GetComponent<DisplayText>().AddText(dialogue, mCurrentPiece);

    }

    public void UpdateEmotion()
    {
        // emotional matrix calcs will go here
        // for now we just set it to neutral
        var EmotionalResult = AllEmotions[Random.Range(0, AllEmotions.Length)];

        // Adjust character EASA values here (in a separate function perhaps?)
        // To display any emotional change...
        //TextBox.GetComponent<DisplayText>().EmotionChange(mCurrentPiece, EmotionalResult);

        CharacterPortrait.GetComponent<CharacterSpriteDisplay>().ShowCorrectEmotion(EmotionalResult);

        // full list of emotional options follows according to the filenames of the faces
        //Aggressive
        //Alarm
        //Anger
        //Anxiety
        //Aware
        //Contempt
        //Curiosity
        //Cynicism
        //Delight
        //Despair
        //Disappointed
        //Disbelief
        //Disgust
        //Distracted
        //Dominance
        //Envy
        //Fear
        //Guilty
        //Happy
        //Love
        //Morbidity
        //Neutral
        //Nostalgic
        //Optimism
        //Outrage
        //Pessimistic
        //Pride
        //Remorse
        //Sad
        //Shame
        //Sorrow
        //Submission
        //Trust
    }

    public void EndConversation()
    {
        TextBox.GetComponent<DisplayText>().ConversationEnder(mCurrentPiece);

        // activate the close button here?
    }

    public void CloseConversation()
    {
        mDirector.GetComponent<Director>().AllConversationsEnded();
    }

    public void NextConversation()
    {
        mDirector.GetComponent<Director>().ContinueConversationScene(this);
    }

    
}
