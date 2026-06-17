window.focusLastTextAreaInCard = function (cardId) {
    const card = document.getElementById(cardId);
    if (!card) return;
    const textAreas = card.querySelectorAll('fluent-text-area');
    if (!textAreas.length) return;
    const last = textAreas[textAreas.length - 1];
    if (last && last.shadowRoot) {
        const inner = last.shadowRoot.querySelector('textarea');
        if (inner) inner.focus();
    }
};
