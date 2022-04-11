# Unity Mediation Toolset

## Interstitial:

```csharp
    public class LoseInterstitialAd : InterstitialAd
    {
    }
```

## Rewarded

```csharp
    public class DoubleCoinsReardedAd : RewardedAd
    {
        protected override void OnUserRewarded(object sender, RewardEventArgs args)
        {
            base.OnUserRewarded(sender, args);
            GameData.AdTimestamp = new DateTimeOffset(DateTime.UtcNow.AddSeconds(_adCooldown)).ToUnixTimeSeconds();
            GiftCoinsScreen.Show();
        }
    }
```

Usage:

```csharp
DoubleCoinsReardedAd.Show();
```