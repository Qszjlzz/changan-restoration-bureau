using UnityEngine;

namespace ChanganRestorationBureau
{
    public sealed class ProofNpcInteractable : MonoBehaviour
    {
        public string npcId;
        public string displayName;
        public Sprite portraitSprite;

        public bool CanInteract(ProofDayState dayState, ProofObjectiveState objective)
        {
            switch (npcId)
            {
                case "stele_du":
                    return objective != null && objective.Sampled;
                default:
                    return true;
            }
        }

        public void BeginInteraction(ProofDayState dayState, ProofObjectiveState objective, ProofDialogueController dialogue)
        {
            if (dialogue == null)
            {
                return;
            }

            var conversation = BuildConversation(dayState, objective);
            if (conversation == null)
            {
                return;
            }

            dialogue.BeginConversation(conversation);
        }

        public string BuildPrompt(ProofDayState dayState, ProofObjectiveState objective)
        {
            switch (npcId)
            {
                case "han_niangzi":
                    if (dayState != null && !dayState.CommissionAccepted)
                    {
                        return "Press E to take the commission";
                    }

                    if (dayState != null && objective != null && objective.Repaired && !dayState.OutcomeResolved)
                    {
                        if (dayState.SelectedRestorationBranch == RestorationBranch.QuickReuse)
                        {
                            return "Press E to return the restored tile";
                        }

                        if (dayState.SelectedRestorationBranch == RestorationBranch.CarefulExhibit && objective.Displayed)
                        {
                            return "Press E to resolve the exhibition";
                        }
                    }

                    return "Press E to talk to Han Niangzi";
                case "apprentice_dou":
                    return dayState != null && dayState.OutcomeResolved && !dayState.DaySummaryShown ? "Press E to read the day ledger" : "Press E to talk to Apprentice Dou";
                case "stele_du":
                    return "Press E to consult Stele Rubbing Du";
                default:
                    return "Press E to talk";
            }
        }

        private ProofConversation BuildConversation(ProofDayState dayState, ProofObjectiveState objective)
        {
            switch (npcId)
            {
                case "han_niangzi":
                    return BuildHanConversation(dayState, objective);
                case "apprentice_dou":
                    return BuildDouConversation(dayState, objective);
                case "stele_du":
                    return BuildDuConversation(dayState, objective);
                default:
                    return new ProofConversation
                    {
                        speakerName = displayName,
                        portrait = portraitSprite,
                        lines = new[] { "There is nothing to say here yet." }
                    };
            }
        }

        private ProofConversation BuildHanConversation(ProofDayState dayState, ProofObjectiveState objective)
        {
            if (dayState == null)
            {
                return null;
            }

            if (!dayState.CommissionAccepted)
            {
                return new ProofConversation
                {
                    speakerName = "Han Niangzi",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "This tile was part of my family's old stall front.",
                        "After the fire, we kept what we could. I do not know whether to use it again or keep it safe.",
                        "Will you take a look for me?"
                    },
                    onComplete = dayState.AcceptCommission
                };
            }

            if (objective != null
                && objective.Repaired
                && dayState.SelectedRestorationBranch == RestorationBranch.QuickReuse
                && !dayState.OutcomeResolved)
            {
                return new ProofConversation
                {
                    speakerName = "Han Niangzi",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "You kept its wear, but made it strong enough to return.",
                        "That is how our market survives. We mend what we can and keep the stall front breathing."
                    },
                    onComplete = () => dayState.ResolveOutcome("quick_reuse")
                };
            }

            if (objective != null
                && objective.Displayed
                && dayState.SelectedRestorationBranch == RestorationBranch.CarefulExhibit
                && !dayState.OutcomeResolved)
            {
                return new ProofConversation
                {
                    speakerName = "Han Niangzi",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "The pattern is clear again. It still carries the market smoke, but now it also carries your care.",
                        "Let us keep it where people can see what survived."
                    },
                    onComplete = () => dayState.ResolveOutcome("careful_exhibit")
                };
            }

            if (dayState.OutcomeResolved)
            {
                var resolvedQuickReuse = string.Equals(dayState.ResolvedOutcomeId, "quick_reuse", System.StringComparison.OrdinalIgnoreCase);
                return new ProofConversation
                {
                    speakerName = "Han Niangzi",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        resolvedQuickReuse
                            ? "Thank you. It can stand with us again, and that matters."
                            : "Thank you. It feels lighter now, whatever shape its future takes."
                    }
                };
            }

            if (objective != null && objective.Sampled && !objective.Repaired)
            {
                return new ProofConversation
                {
                    speakerName = "Han Niangzi",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "You found it? Good. The soot always made the pattern hard to read.",
                        "When you decide, I will trust your judgment."
                    }
                };
            }

            if (objective != null
                && objective.Repaired
                && dayState.SelectedRestorationBranch == RestorationBranch.CarefulExhibit
                && !objective.Displayed)
            {
                return new ProofConversation
                {
                    speakerName = "Han Niangzi",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "If you wish to keep it for display, let me see it in the bureau case first.",
                        "I want to understand what others will see when they look at it."
                    }
                };
            }

            return new ProofConversation
            {
                speakerName = "Han Niangzi",
                portrait = portraitSprite,
                lines = new[]
                {
                    "The lotus tile should still be near the old salvage heap.",
                    "Please tell me what kind of future it can still have."
                }
            };
        }

        private ProofConversation BuildDouConversation(ProofDayState dayState, ProofObjectiveState objective)
        {
            if (dayState == null)
            {
                return null;
            }

            if (!dayState.CommissionAccepted)
            {
                return new ProofConversation
                {
                    speakerName = "Apprentice Dou",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "Master, Han Niangzi is looking for you at the night market.",
                        "She brought something from an old stall front. It sounds important."
                    }
                };
            }

            if (dayState.OutcomeResolved && !dayState.DaySummaryShown)
            {
                return new ProofConversation
                {
                    speakerName = "Apprentice Dou",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "That settles the commission for today.",
                        "Come, let us write the result into the day ledger.",
                        dayState.BuildSummaryText()
                    },
                    onComplete = dayState.ShowDaySummary
                };
            }

            if (objective != null && objective.Sampled && !objective.Repaired)
            {
                return new ProofConversation
                {
                    speakerName = "Apprentice Dou",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "The tile can be cleaned now.",
                        "After that, you must decide whether to ready it for use or conserve it properly."
                    },
                    onComplete = dayState.MarkRestorationReady
                };
            }

            if (objective != null && objective.Repaired && !dayState.OutcomeResolved)
            {
                var reminder = dayState.SelectedRestorationBranch == RestorationBranch.QuickReuse
                    ? "Han Niangzi is waiting at the market so the tile can return to use."
                    : "Set the tile on the display stand so Han can judge the exhibit path.";
                return new ProofConversation
                {
                    speakerName = "Apprentice Dou",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        reminder
                    }
                };
            }

            if (dayState.DaySummaryShown)
            {
                return new ProofConversation
                {
                    speakerName = "Apprentice Dou",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "Tomorrow's work will be easier if we keep both the tools and the stories in order."
                    }
                };
            }

            return new ProofConversation
            {
                speakerName = "Apprentice Dou",
                portrait = portraitSprite,
                lines = new[]
                {
                    "The market awning with the red trim. She is waiting there."
                }
            };
        }

        private ProofConversation BuildDuConversation(ProofDayState dayState, ProofObjectiveState objective)
        {
            if (dayState == null)
            {
                return null;
            }

            if (objective == null || !objective.Sampled)
            {
                return new ProofConversation
                {
                    speakerName = "Stele Rubbing Du",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "Bring me the object itself, and I may tell you more."
                    }
                };
            }

            if (!dayState.HasConsultedSteleDu && dayState.WorkHours > 0)
            {
                return new ProofConversation
                {
                    speakerName = "Stele Rubbing Du",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "The soot line is wrong for simple weathering.",
                        "This tile was scorched, then set back into use later. Someone chose survival over symmetry.",
                        "If you restore it, decide whether you are preserving a roof tile or a neighborhood memory."
                    },
                    onComplete = dayState.MarkConsultedSteleDu
                };
            }

            if (!dayState.HasConsultedSteleDu && dayState.WorkHours <= 0)
            {
                return new ProofConversation
                {
                    speakerName = "Stele Rubbing Du",
                    portrait = portraitSprite,
                    lines = new[]
                    {
                        "You already know enough to act. Spend the rest of the day at the bench, not here."
                    }
                };
            }

            return new ProofConversation
            {
                speakerName = "Stele Rubbing Du",
                portrait = portraitSprite,
                lines = new[]
                {
                    "Fire broke it. Reuse gave it a second life. Your repair should respect that choice."
                }
            };
        }
    }
}
