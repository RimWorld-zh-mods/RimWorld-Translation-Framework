using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Verse;

namespace RimWorld {
    /// <summary>
    /// For combine random name to solid name.
    /// </summary>
    public class CombineNameDef : Def {

        #region Fields

        public GenderPossibility gender;

        public int amount = 1024;

        public List<RandomNameDef> firstRandomNameDefs;

        public List<RandomNameDef> lastRandomNameDefs;

        public bool useNickNameDatabase;

        public float nickNameDatabaseChance = 0.125f;

        #endregion

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (this.amount <= 0) {
                yield return $"amount({amount}) cannot be less than or equal 0.";
            }
            if (this.firstRandomNameDefs == null || this.firstRandomNameDefs.Count == 0) {
                yield return "the field 'firstRandomNameDefs' cannot be null or empty.";
            }
            if (this.lastRandomNameDefs == null || this.lastRandomNameDefs.Count == 0) {
                yield return "the field 'lastRandomNameDefs' cannot be null or empty.";
            }
            if (this.nickNameDatabaseChance <= 0f) {
                yield return $"nickNameDatabaseChance({nickNameDatabaseChance}) cannot be less than or equal 0.";
            }
            if (this.nickNameDatabaseChance > 1.0f) {
                yield return $"nickNameDatabaseChance({nickNameDatabaseChance}) cannot be greater than 1.";
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{this.defName}'";
        }

        public IEnumerable<NameTriple> ToNameTriples() {
            if (this.amount > 0 &&
                this.nickNameDatabaseChance > 0f &&
                this.nickNameDatabaseChance <= 1.0f &&
                this.firstRandomNameDefs != null &&
                this.lastRandomNameDefs != null) {
                IEnumerable<string> firstNames = from def in this.firstRandomNameDefs
                                                 from str in def.shuffledNames
                                                 select str;
                IEnumerable<string> lastNames = from def in this.lastRandomNameDefs
                                                from str in def.shuffledNames
                                                select str;
                HashSet<string> nameSet = new HashSet<string>();
                NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
                for (int i = 0; i < this.amount; i++) {
                    foreach (string last in lastNames) {
                        //Thread.Sleep(1);
                        string first = firstNames.RandomElement();
                        int threshold = 0;
                        while (!nameSet.Add($"{first}")) {
                            first = firstNames.RandomElement();
                            threshold++;
                            if (threshold > 99)
                                break;
                        }
                        string nick = null;
                        if (Rand.Chance(this.nickNameDatabaseChance)) {
                            float f = Rand.Value;
                            Gender gender;
                            if (this.gender == GenderPossibility.Male) {
                                if (f < 0.66666666f) {
                                    gender = Gender.Male;
                                } else {
                                    gender = Gender.None;
                                }
                            } else if (this.gender == GenderPossibility.Female) {
                                if (f < 0.66666666f) {
                                    gender = Gender.Female;
                                } else {
                                    gender = Gender.None;
                                }
                            } else {
                                if (f < 0.33333333f) {
                                    gender = Gender.Male;
                                } else if (f < 0.66666666f) {
                                    gender = Gender.Female;
                                } else {
                                    gender = Gender.None;
                                }
                            }
                            nick = nameBank.GetName(PawnNameSlot.Nick, gender);
                        }
                        NameTriple name = new NameTriple(first, nick, last);
                        name.ResolveMissingPieces();
                        yield return name;
                        i++;
                    }
                }
            }
        }
    }
}
