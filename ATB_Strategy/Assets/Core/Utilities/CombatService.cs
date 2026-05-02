using UnityEngine;

public static class CombatService
{
    private static readonly int HEIGHT_ADVANTAGE_BONUS_ACCURACY = 5;
    
    private static readonly float MELEE_DISTANCE_THRESHOLD = 5f;
    private static readonly float MEDIUM_DISTANCE_THRESHOLD = 15f;
    private static readonly float RANGE_DISTANCE_THRESHOLD = 20f;

    private static readonly float MAX_DISTANCE_FACTOR = 1f;
    private static readonly float MIN_DISTANCE_FACTOR = -0.5f;
    
    private static readonly float LOW_COVER_ANGLE_VERTICAL_THRESHOLD = 45f;
    private static readonly float FULL_COVER_ANGLE_VERTICAL_THRESHOLD = 60f;

    private static readonly int LOW_COVER_BONUS_DODGE = 30;
    private static readonly int FULL_COVER_BONUS_DODGE = 60;
    
    public static CombatContext CalculateHitContext(CombatObject dealer, int accuracyPercent, WeaponController weapon, CombatTarget target)
    {
        CombatContext context = new CombatContext();

        float distance = Vector3.Distance(target.Target.Position, dealer.Position);
        float distanceFactor = CalculateDistanceFactor(distance, weapon.RangeType);
        float heightDiff = dealer.Position.y - target.Target.Position.y;
        int heightAdvantage = Mathf.FloorToInt(Mathf.Abs(heightDiff)) * (int)Mathf.Sign(heightDiff);
        
        float hitChance =
            (accuracyPercent
             + weapon.Accuracy * distanceFactor
             + (heightAdvantage * HEIGHT_ADVANTAGE_BONUS_ACCURACY))
            * target.VisionPercent
            - target.Target.GetDodge(dealer.Position);
        
        float critChance = Mathf.Clamp01(hitChance - 100) + weapon.CritChance * distanceFactor;

        context.HitChance = Mathf.CeilToInt(Mathf.Clamp(hitChance, 1, 100));
        context.CritChance = Mathf.CeilToInt(Mathf.Clamp(critChance, 0, 100));
        
        context.Dealer = dealer;
        context.Target = target;

        context.Damage = weapon.Damage;
        context.CritDamage = weapon.CritDamage;

        return context;
    }

    public static bool CalculateHit(CombatContext context, out HitResult result)
    {
        int random = Random.Range(0, 100);

        result = new HitResult();
        result.Dealer = context.Dealer;
        
        if (random < context.HitChance)
        {
            random = Random.Range(0, 100);
            if (random < context.CritChance)
            {
                result.IsCritical = true;
                result.Damage = Random.Range(context.CritDamage.Min, context.CritDamage.Max + 1);
            }
            else
            {
                result.IsCritical = false;
                result.Damage = Random.Range(context.Damage.Min, context.Damage.Max + 1);
            }

            return true;
        }

        return false;
    }

    private static float CalculateDistanceFactor(float distance, WeaponRangeType type)
    {
        switch (type)
        {
            case(WeaponRangeType.Melee): return CalculateDistanceFactorMelee(distance);
            case(WeaponRangeType.Medium): return CalculateDistanceFactorMedium(distance);
            case(WeaponRangeType.Ranged): return CalculateDistanceFactorRange(distance);
        }
        
        return MAX_DISTANCE_FACTOR;
    }

    private static float CalculateDistanceFactorMelee(float distance)
    {
        if (distance <= MELEE_DISTANCE_THRESHOLD)
        {
            return MAX_DISTANCE_FACTOR;
        }
        
        if (distance >= MEDIUM_DISTANCE_THRESHOLD)
        {
            return MIN_DISTANCE_FACTOR;
        }
        
        float f = (distance - MELEE_DISTANCE_THRESHOLD) / (MEDIUM_DISTANCE_THRESHOLD - MELEE_DISTANCE_THRESHOLD);
        float distanceFactor = Mathf.Lerp(MAX_DISTANCE_FACTOR, MIN_DISTANCE_FACTOR, f);

        return distanceFactor;
    }
    
    private static float CalculateDistanceFactorMedium(float distance)
    {
        if (distance <= MELEE_DISTANCE_THRESHOLD)
        {
            float f = distance / MELEE_DISTANCE_THRESHOLD;
            float distanceFactor = Mathf.Lerp(MIN_DISTANCE_FACTOR, MAX_DISTANCE_FACTOR, f);

            return distanceFactor;
        }
        
        if (distance >= MEDIUM_DISTANCE_THRESHOLD)
        {
            float f = (distance - MEDIUM_DISTANCE_THRESHOLD) / (RANGE_DISTANCE_THRESHOLD - MEDIUM_DISTANCE_THRESHOLD);
            float distanceFactor = Mathf.Lerp(MAX_DISTANCE_FACTOR, MIN_DISTANCE_FACTOR, f);

            return distanceFactor;
        }

        return MAX_DISTANCE_FACTOR;
    }
    
    private static float CalculateDistanceFactorRange(float distance)
    {
        if (distance <= MELEE_DISTANCE_THRESHOLD)
        {
            return MIN_DISTANCE_FACTOR;
        }
        
        if (distance >= MEDIUM_DISTANCE_THRESHOLD)
        {
            return MAX_DISTANCE_FACTOR;
        }
        
        float f = (distance - MELEE_DISTANCE_THRESHOLD) / (MEDIUM_DISTANCE_THRESHOLD - MELEE_DISTANCE_THRESHOLD);
        float distanceFactor = Mathf.Lerp(MIN_DISTANCE_FACTOR, MAX_DISTANCE_FACTOR, f);

        return distanceFactor;
    }

    public static int CalculateCoverDodge(GridTile tile, int targetDodge, Vector3 dealerPos)
    {
        Vector3 tilePos = GridParameters.LevelGrid.GetTileWorldPos(tile);

        Vector3 fullDirection = dealerPos - tilePos;
        Vector3 flatDirection = new Vector3(fullDirection.x, 0, fullDirection.z).normalized;
        
        int bestCoverIndex = -1;
        float bestDot = float.MinValue;

        for (int i = 0; i < GridParameters.COVER_DIRECTIONS.Length; i++)
        {
            if (tile.Covers[i] == TileCover.None) continue;

            float dot = Vector3.Dot(flatDirection, GridParameters.COVER_DIRECTIONS[i]);

            if (dot >= 0.5f && (bestCoverIndex == -1 || tile.Covers[i] >= tile.Covers[bestCoverIndex]))
            {
                bestDot = dot;
                bestCoverIndex = i;
            }
        }

        if (bestCoverIndex == -1)
            return targetDodge;

        TileCover cover = tile.Covers[bestCoverIndex];
        
        float verticalAngle = Vector3.Angle(fullDirection, new Vector3(fullDirection.x, 0f, fullDirection.z));

        if (cover == TileCover.Low && verticalAngle > LOW_COVER_ANGLE_VERTICAL_THRESHOLD)
            return targetDodge;

        if (cover == TileCover.Full && verticalAngle > FULL_COVER_ANGLE_VERTICAL_THRESHOLD)
            return targetDodge;

        return cover switch
        {
            TileCover.Low  => targetDodge + LOW_COVER_BONUS_DODGE,
            TileCover.Full => targetDodge + FULL_COVER_BONUS_DODGE,
            _ => targetDodge
        };
    }
}

public struct CombatContext
{
    public CombatObject Dealer;
    public CombatTarget Target;

    public int HitChance;
    public int CritChance;

    public RangeIntStat Damage;
    public RangeIntStat CritDamage;
}

public struct HitResult
{
    public CombatObject Dealer;
    public int Damage; 
    public bool IsCritical;
}
