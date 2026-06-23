function togglePasswordVisibility(btn) {
    const field = btn.closest('fluent-text-field');
    const input = field?.shadowRoot?.querySelector('input');
    if (!input) return;
    const show = input.type === 'password';
    input.type = show ? 'text' : 'password';
    btn.querySelector('.eye-show').style.display = show ? 'none' : 'inline-flex';
    btn.querySelector('.eye-hide').style.display = show ? 'inline-flex' : 'none';
}
