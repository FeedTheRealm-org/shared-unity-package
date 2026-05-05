using System.Collections.Generic;
using API;
using UnityEngine;
using UnityEngine.UIElements;

namespace FTRShared.UI.ZoneStatusBadge
{
    [RequireComponent(typeof(UIDocument))]
    public class ZoneStatusBadgeController : MonoBehaviour
    {
        public enum State
        {
            Online,
            Degraded,
            Offline,
        }

        private const string ClassBase = "zone-status-badge";
        private const string ClassOnline = "zone-status-badge--online";
        private const string ClassDegraded = "zone-status-badge--degraded";
        private const string ClassOffline = "zone-status-badge--offline";

        private readonly string[] StateClasses = { ClassOnline, ClassDegraded, ClassOffline };

        private readonly string[] StateTexts = { "Online", "Degraded", "Offline" };

        [SerializeField]
        public VisualTreeAsset Template;

        public VisualElement Create(State state)
        {
            VisualElement root = Template != null ? Template.Instantiate() : BuildFallback();

            var badge = root.Q<Label>("ZoneStatusBadge") ?? root as Label;
            ApplyState(badge, state);
            return root;
        }

        public void Apply(Label badge, State state)
        {
            StopBlink(badge);
            ApplyState(badge, state);
        }

        public void Apply(Label badge, bool isOnline) =>
            Apply(badge, isOnline ? State.Online : State.Offline);

        public void Apply(Label badge, List<WorldZoneMetadata> zones) =>
            Apply(badge, Evaluate(zones));

        public State Evaluate(List<WorldZoneMetadata> zones)
        {
            if (zones == null || zones.Count == 0)
                return State.Offline;

            int online = 0;
            foreach (var z in zones)
                if (z.is_online)
                    online++;

            if (online == 0)
                return State.Offline;
            if (online == zones.Count)
                return State.Online;
            return State.Degraded;
        }

        public void StopBlink(Label badge)
        {
            if (badge?.userData is IVisualElementScheduledItem handle)
            {
                handle.Pause();
                badge.userData = null;
            }
        }

        private void ApplyState(Label badge, State state)
        {
            if (badge == null)
            {
                Debug.LogWarning(
                    "[ZoneStatusBadge] Target badge element is null. Ensure the template contains a Label named 'ZoneStatusBadge' or a valid badge was provided."
                );
                return;
            }

            foreach (var cls in StateClasses)
                badge.RemoveFromClassList(cls);

            badge.AddToClassList(
                state switch
                {
                    State.Online => ClassOnline,
                    State.Degraded => ClassDegraded,
                    _ => ClassOffline,
                }
            );

            var text = badge.Q<Label>("BadgeText");
            if (text != null)
                text.text = StateTexts[(int)state];

            var dot = badge.Q<Label>("BadgeDot");
            if (dot != null)
                StartBlink(badge, dot);
        }

        private void StartBlink(Label badge, Label dot)
        {
            StopBlink(badge);

            bool visible = true;
            var handle = dot
                .schedule.Execute(() =>
                {
                    visible = !visible;
                    dot.style.opacity = visible ? 1f : 0f;
                })
                .Every(600);

            badge.userData = handle;
        }

        private VisualElement BuildFallback()
        {
            Debug.LogWarning(
                "[ZoneStatusBadge] UXML template not found. "
                    + "Assign ZoneStatusBadge.Template. Falling back to a programmatically built badge"
            );

            var badge = new Label { name = "ZoneStatusBadge" };
            badge.AddToClassList(ClassBase);

            var dot = new Label { name = "BadgeDot" };
            dot.AddToClassList("zone-status-badge__dot");

            var text = new Label { name = "BadgeText" };
            text.AddToClassList("zone-status-badge__text");

            badge.Add(dot);
            badge.Add(text);
            return badge;
        }
    }
}
