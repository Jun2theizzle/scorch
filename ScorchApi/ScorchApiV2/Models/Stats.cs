﻿using Amazon.DynamoDBv2.DocumentModel;

namespace ScorchApiV2.Models
{
    public class Stats
    {
        public int Strength     { get; set; }
        public int Dexterity    { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom       { get; set; }
        public int Charisma     { get; set; }

        public Stats() { }
    }
}