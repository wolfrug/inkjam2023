INCLUDE inkjam2023_functions.ink
->start

==toDoList
{fixBreaker:<s>}\- Fix the breaker<>{fixBreaker:</s>}
{fixBreaker:
{makeFood.options:<s>}\- Go to the kitchen<>{makeFood.options:</s>}
}
{makeFood.options:
{makeFood.finish:<s>}\- Make the Special Bait for your brother.<>{makeFood.finish:</s>}
}
{makeFood.finish:
\- Feed the Special Bait to your brother.<>
}
{notesTaken>0: <br><br>\- Notes Found: {notesTaken} / 14}
->END

==start
You awaken to utter darkness from a horrible nightmare. The man in the cap was there. He was hunting you.

The lights have gone out. Again. #lightningStrike

+ [Grab the flashlight.]
PLAY_SFX(SFX_LIGHTON) #lightOn
- You turn on the flashlight, illuminating the darkness of your home. It smells of lost glories.

In the distance, you hear your brother's moan. PLAY_SFX(SFX_ZOMBIE_MOAN) #interact_1

+ [I better be careful in the dark.]
- He's easy enough to slip past when there's light, but in the dark, accidents happen so easily.

That's how it happened for him, after all. Just one moment of inattentiveness... PLAY_SFX(SFX_ZOMBIE_MOAN)

...you better see to the breaker. You remember it's by the main door in the hall. #openNotes #updateToDoList 

<color=green>[Use the mouse to look around, WASD or arrow keys to move. Use E to interact. Watch out for your brother. He is feeling...under the weather.]</color>
->DONE


==fixBreaker
You look at the sparking mess with despair. This isn't going to work, is it?

+ [Have a try] #interact_2
You try all of the switches. One by one they fail. The very last one you flip seems to do something, though. It's marked 'kitchen', written by hand on a faded piece of white tape.

The rest of the house is staying dark, but maybe the kitchen at least has a light. PLAY_SFX(SFX_ZOMBIE_MOAN)

Your brother is getting hungry, and getting into places he's not supposed to. He could probably use a snack...

The kitchen should have everything you need, and now you should be able to see what you're doing too.

...just need to be careful around your brother... #updateToDoList
->DONE

==makeFood
{not fixBreaker:
There's nothing you can do here, not in the total dark. You need both hands to work the kitchen. Best see to that breaker...
->END
}
{not options:
You like to make his food in these sacks. They're easy to carry and place wherever you need.

You make it with your own recipe: Flour, as a binding agent. Meat, for example from roadkill. Pigs' blood. And of course, medicine.

Even in his current state, he still needs his medicine...
- else:
You return to the unfinished bag of bait.
}

- (options) #updateToDoList

* (flour) [{IsInteractable(ingredients?Flour)} Add the Flour.]
~ingredients-=Flour
You pour the flour liberally into the mix. It reminds you of the time your mother tried to teach you how to bake.

'It'll be important in the coming world. Baking. It's a survival skill.'

She wasn't wrong, your mother, bless her.
* (roadkill) [{IsInteractable(ingredients?Roadkill)} Add the Roadkill.]
~ingredients-=Roadkill
You know you should skin it, remove the bones and the guts and everything else that might be dangerous, but right now you can't be bothered.

The crunchier it is, the longer it will take him to finish it, after all.
* (blood) [{IsInteractable(ingredients?PigsBlood)} Add the Blood.]
~ingredients-=PigsBlood
You pour in the blood, kept liquid thanks to the anti-coagulants swimming around in there. He's lost most of his senses, but not his sense of smell.

You wonder if there's something in pig's blood in particular that attracts him. You recall the taste of human, according to certain cannibals, was akin to pork. Long pig, is what human flesh was called. Maybe there's a connection?
* (medicine) [{IsInteractable(ingredients?Medicine)} Add the Medicine.]
~ingredients-=Medicine
You carefully dole out the pills one by one: worm medicine, meant for cats. Antibiotics (for dogs). And ample amounts of crushed xanax - for sleep.

After eating this, he should be full and ready for a nap...
+ [Leave.]
Time to go ingredient-hunting...
->DONE
- 
{flour && blood && roadkill && medicine:
->finish
- else:
->options
}

=finish
That was the last ingredient. You stir it all into the sack. It's hard, heavy work. Blood begins to seeps out through the canvas.

This ought to do it. Now just to lure him back into his room. Once he starts feasting on the food, he'll leave you alone. Of course you could let him eat it anywhere...it's just difficult to drag him around after he's knocked out.

+ [Tie up the sack.] #spawnSpecialBait #updateToDoList
->DONE

==ingredient
=medicine
You don't remember when exactly you decided to start keeping all of your pills in a big pill-pile in your bathtub. You're sure you didn't do it before your brother got bit.

* {makeFood.options} [Dig around for ingredients.]
You fish around the pile of pills, flashlight in one hand, finding the right pills by size, color and weight. You're an old hand at this by now.
~ingredients+=Medicine
<color=green>[You found Medicine.]</color>
->DONE
+ [Leave]
You leave the pile of pills to its fate.
->DONE
=flour
A sack of flour. When you open it, you notice it is full of mealworms. That's okay. You weren't planning on eating it anyway.

* {makeFood.options} [Collect a scoop.]
You pick up a nearby measuring cup and stick it into the flour, making sure you pluck as many mealworms as you can while you're at it.

Your brother won't mind. In fact he'll do the opposite.
~ingredients+=Flour
<color=green>[You found Flour.]</color>
->DONE
+ [Leave]
You leave the mealworms to their meal.
->DONE
=roadkill
{not taken:
You stare at the bloody rug on the coffee table, at a loss for why it's there. Then you remember.

The...animal...that you found out on the road. Roadkill. Maybe a fox.
- else:
Just the bloodstains remain on the table now. Those will be hard to get out.
}
* (taken) {makeFood.options} [Wrap it in the rug and take it with you.] #removeRoadkill
You wrap it in the rug, surprised that it doesn't smell worse than it does. It's not very heavy.
~ingredients+=Roadkill
<color=green>[You found Roadkill!]</color>
->DONE
+ [Leave]
Was this really the best place to keep it? You're not sure.
->DONE

=blood
Barrels. Why aren't they in the storeroom? You'll move them tomorrow, for sure. {not open: Now what was it you stored in them again?|The blood barely reflects your flashlight.}

* (open) {makeFood.options} [Open the barrel.] #openBarrel
Ah yes. This is where you left your pig's blood. No wonder you didn't take it all the way to the storerooms. Blood is heavy.

There is a small bucket inside. You take it and fill it with blood. You can almost taste the smell.
~ingredients+=PigsBlood
<color=green>[You found Pig's Blood.]</color>
->DONE
+ [Leave]
A mystery for another time.
->DONE

==defeat
He takes you by the shoulders, his fingers cold and clammy. His reflect nothing, they are just dark pits. You try to struggle, but he's too strong. PLAY_SFX(SFX_DEATHGROAN) #deathVFX

It is all over in the blink of an eye. All it took was one moment of not paying attention...

+ [Restart]
You lost... #restart
->END
==victory
The drugs take effect, and slowly but surely your brother's limbs begin to weaken and losen, and before he finishes the last bite he collapses.

He'll rest now, finally, and so can you. Tomorrow is a new day. He'll get better by tomorrow. You're sure of it...

+ [Finish.] #winGame
You win!
->END

==descriptions
=bait
{bait<2:
Some bait. It won't last long, but it might buy me some time. SET_TEXTBOX(playerBox, current)
}
->END
=storeroom
{storeroom<2:
One of your storerooms, in desperate need of inventory. SET_TEXTBOX(playerBox, current)
}
->DONE
=kitchen
{kitchen<2:
The kitchen. Could use some utensils. SET_TEXTBOX(playerBox, current)
}
->DONE
=outsidedoor
Yep, still locked and barred. No-one coming in this way... SET_TEXTBOX(playerBox, current)
->DONE
=backdoor
Wait...was someone there? The man in the cap? ...no. SET_TEXTBOX(playerBox, current)
->DONE
=hallway
{I need to clean this place... |This place could use a broom... SET_TEXTBOX(playerBox, player)}
->DONE
=bed
Your brother's old bed...you really ought to clean it up. SET_TEXTBOX(playerBox, current)
->END
=bath
He stumbled here when the sickness started to get bad... SET_TEXTBOX(playerBox, current)
->END
=breaker
Damn thing keeps going on the fritz...
->END
=jasonsroom
Jason's new room. It doesn't smell great, but it's easy enough to camouflage when needed... SET_TEXTBOX(playerBox, player)
->END

==notes
~notesTaken++
#updateToDoList
{~->n1|->n2|->n3|->n4|->n5|->n6|->n7|->n8|->n9|->n10|->n11|->n12|->n13|->n14}
=n1
"October 4. Jason and I finished getting ready for lockdown. The talking heads on the news seem to think this will all be over by spring, but we all know how that turned out last time..."  SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n2
"October 8. Notes to self: the sick are very slow, have shit sight, can't stop moaning and groaning, smell really bad, and are constantly hungry. Yuck." SET_TEXTBOX(noteBox, current)PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n3
"October 18. Week two of lockdown. The talking heads insist a vaccine is just around the corner. Some new mRNA thing that absolutely won't give you cancer or autism." SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n4
"November 1. Saw someone moving around out there. Maybe a neighbour. Maybe an opportunist. Jason went out to tell him to find his own mansion to squat in." SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n5
"November 2. Shit. Fuck. Shit fuck ass. It wasn't a neighbour. Or it was, but the wrong kind. Don't they know they have to ISOLATE when they've been...ah shit." SET_TEXTBOX(noteBoxBlood, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n6
"November 15. He begged me to kill him at the end, but the talking heads said the vaccine was just around the corner. It was just the pain talking I think." SET_TEXTBOX(noteBoxBlood, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n7
"November...? Moved Jason to his new room, away from the windows. He gets too excited seeing the great outdoors, always wants to escape. Can't have that. Isolate!" SET_TEXTBOX(noteBoxBlood, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n8
"December...? Got Jason a Christmas present. It's probably Christmas. A live bunny. It kept him occupied for hours in his room. Fewer talking heads on the TV these days." SET_TEXTBOX(noteBoxBlood, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n9
"January...? Just heard on the radio the vaccines are being shipped out now. Nice. Except they say only the uninfected will be dosed. Boo. I guess me and Jason will have to wait." SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n10
"February...? Running a little low on supplies, but not much left of winter now. Weather is getting really shitty though, and power is a bit spotty. But so far so good." SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n11
"March...? I thought I saw someone outside again. A man in a cap. I think he might have had a gun. Fuck. Can't let him see Jason. There are all kinds of crazies out there these days." SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n12
"April...? Heard they are airdropping the vaccine now. Local community got a shipment. Guess it's finally time for me to go for a little trip. Just hope Jason stays safe while I'm gone..." SET_TEXTBOX(noteBox, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n13
"May...? It didn't work. I injected him. Full dose. Gave him my dose too. Still nothing. Maybe...maybe it just needs more time, because he was infected so long? That's probably it. He just needs more time..." SET_TEXTBOX(noteBoxBlood, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
=n14
"I saw the man in the cap again. I'm sure of it. But I'm also sure Jason is talking again. I can leave the door open most days now. He's getting better. Slowly but surely." SET_TEXTBOX(noteBoxBlood, current) PLAY_SFX(SFX_TAKE_NOTE)
->DONE
