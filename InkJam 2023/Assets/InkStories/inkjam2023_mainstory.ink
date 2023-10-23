INCLUDE inkjam2023_functions.ink
->start

==toDoList
{fixBreaker:<s>}\- Fix the breaker<><br>{fixBreaker:</s>}
{fixBreaker:
{makeFood.options:<s>}\- Go to the kitchen<><br>{makeFood.options:</s>}
}
{makeFood.options:
{makeFood.finish:<s>}\- Make the Special Bait for your brother.<><br>{makeFood.finish:</s>}
}
{makeFood.finish:
\- Feed the Special Bait to your brother.<><br>
}
->END

==start
You awaken to utter darkness from a horrible nightmare.

The lights have gone out. Again. #lightningStrike

+ [Grab the flashlight.]
PLAY_SFX(SFX_LIGHTON) #lightOn
- You turn on the flashlight, illuminating the darkness of your home. It smells of lost glories.

In the distance, you hear your brother's moan. PLAY_SFX(SFX_ZOMBIE_MOAN) #interact_1

+ [I better be careful in the dark.]
- He's easy enough to slip past when there's light, but in the dark, accidents happen so easily.

That's how it happened for him, after all. Just one moment of inattentiveness... PLAY_SFX(SFX_ZOMBIE_MOAN)

...you better see to the breaker. You remember it's by the main door in the hall. #updateToDoList

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

He'll rest now, finally, and so can you. Tomorrow is a new day. A new hope for the cure. It can't be long now...

+ [Finish.] #winGame
You win!
->END

==descriptions
=bait
{bait<2:
Your special concoction. Your brother goes crazy for it. Perfect for if you need to sneak by him.
}
->END
=bed
Your brother's old bed...you really ought to clean it up.
->END
=bath
He stumbled here when the sickness started to get bad...
->END
=breaker
Damn thing keeps going on the fritz...
->END