public class UseLife {
    public float threshold = 100f;
    public Scorer lifeBelowThreshold;
    public Scorer hasLifePacks;

    /* Constructor */
    public UseLife() {
        lifeBelowThreshold = new LifeBelowThreshold();
        hasLifePacks = new HasLifePacks();
    }

    float scoreLife, scoreLPs;
    public float Run(Context context) {
        /* check if both of these scores are above the threshold */
        scoreLife = lifeBelowThreshold.Score(context);
        scoreLPs = hasLifePacks.Score(context);

        if (scoreLife > threshold && scoreLPs > threshold)
        {
            var totalScore = scoreLife + scoreLPs;
            return totalScore;
        }
        return 0f;
    }

}

/* Return the score when life is below the threshold */
public sealed class LifeBelowThreshold : Scorer
{
    // life threshold must be something between 0f and 1f
    public float threshold = .35f;
    new public float score = 200f;

    public override float Score(Context context)
    {
        if (context.player.GetCurrentLifePercent() < threshold)
        {
            return this.score;
        }
        return 0f;
    }
}

/* Return this score when a player has any life packs */
public sealed class HasLifePacks : Scorer
{
    public new float score = 200f;

    public override float Score(Context context)
    {
        if (context.player.currentLifePacks >= 0)
        {
            return this.score;
        }
        return 0f;
    }
}
