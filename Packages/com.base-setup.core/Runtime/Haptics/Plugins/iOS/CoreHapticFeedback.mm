// Native side of Core.Haptics on iOS: plays a HapticPreset through the UIKit feedback generators.
// The table is indexed by the HapticPreset values in HapticPreset.cs; keep both in the same order.
// Unity calls this on the main thread, which UIKit requires.
// Written to be correct with and without ARC: the generators are created once and never released, and the table
// holds plain C functions rather than blocks.

#import <UIKit/UIKit.h>

static UIImpactFeedbackGenerator *CoreHapticFeedback_light;
static UIImpactFeedbackGenerator *CoreHapticFeedback_medium;
static UIImpactFeedbackGenerator *CoreHapticFeedback_heavy;
static UINotificationFeedbackGenerator *CoreHapticFeedback_notification;

static void CoreHapticFeedback_CreateGenerators(void)
{
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        CoreHapticFeedback_light = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
        CoreHapticFeedback_medium = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
        CoreHapticFeedback_heavy = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
        CoreHapticFeedback_notification = [[UINotificationFeedbackGenerator alloc] init];
    });
}

// Each play re-prepares its generator so the next play of the same preset starts with less latency.
static void CoreHapticFeedback_Impact(UIImpactFeedbackGenerator *generator)
{
    [generator impactOccurred];
    [generator prepare];
}

static void CoreHapticFeedback_Notify(UINotificationFeedbackType type)
{
    [CoreHapticFeedback_notification notificationOccurred:type];
    [CoreHapticFeedback_notification prepare];
}

static void CoreHapticFeedback_Light(void) { CoreHapticFeedback_Impact(CoreHapticFeedback_light); }
static void CoreHapticFeedback_Medium(void) { CoreHapticFeedback_Impact(CoreHapticFeedback_medium); }
static void CoreHapticFeedback_Heavy(void) { CoreHapticFeedback_Impact(CoreHapticFeedback_heavy); }
static void CoreHapticFeedback_Success(void) { CoreHapticFeedback_Notify(UINotificationFeedbackTypeSuccess); }
static void CoreHapticFeedback_Failure(void) { CoreHapticFeedback_Notify(UINotificationFeedbackTypeError); }

static void (*const CoreHapticFeedback_Presets[])(void) = {
    CoreHapticFeedback_Light,   // HapticPreset.Light = 0
    CoreHapticFeedback_Medium,  // HapticPreset.Medium = 1
    CoreHapticFeedback_Heavy,   // HapticPreset.Heavy = 2
    CoreHapticFeedback_Success, // HapticPreset.Success = 3
    CoreHapticFeedback_Failure, // HapticPreset.Failure = 4
};

extern "C" void CoreHapticFeedback_Play(int preset)
{
    const int count = (int)(sizeof(CoreHapticFeedback_Presets) / sizeof(CoreHapticFeedback_Presets[0]));
    if (preset < 0 || preset >= count)
        return;

    CoreHapticFeedback_CreateGenerators();
    CoreHapticFeedback_Presets[preset]();
}
