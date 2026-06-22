window.patchInfoTooltip = function (anchorId) {
    const tooltip = document.querySelector('fluent-tooltip[anchor="' + anchorId + '"]');
    if (!tooltip) return;
    const region = tooltip.shadowRoot && tooltip.shadowRoot.querySelector('fluent-anchored-region');
    if (!region) return;
    region.horizontalDefaultPosition = 'end';
    region.horizontalPositioningMode = 'dynamic';
};
