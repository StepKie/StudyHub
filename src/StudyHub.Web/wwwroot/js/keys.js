// Study-screen input: Space and 1-4 go to OnKey, a tap anywhere outside controls goes to OnTap, both on the registered .NET object.
window.studyhubKeys = (() => {
    const SHORTCUTS = [' ', '1', '2', '3', '4'];
    const CONTROLS = 'button, a, input, select, textarea, .mud-appbar';
    let ref = null;

    const isShortcut = e => SHORTCUTS.includes(e.key)
        && !e.ctrlKey && !e.metaKey && !e.altKey
        && !['INPUT', 'TEXTAREA', 'SELECT'].includes(e.target.tagName);

    // Space would otherwise scroll the page or re-activate a focused button, so both phases are swallowed.
    const onKeyDown = e => {
        if (!isShortcut(e)) { return; }
        e.preventDefault();
        if (!e.repeat) { ref.invokeMethodAsync('OnKey', e.key); }
    };
    const onKeyUp = e => { if (isShortcut(e)) { e.preventDefault(); } };
    const onClick = e => {
        if (!e.target.closest(CONTROLS)) { ref.invokeMethodAsync('OnTap'); }
    };

    function unregister() {
        document.removeEventListener('keydown', onKeyDown);
        document.removeEventListener('keyup', onKeyUp);
        document.removeEventListener('click', onClick);
        ref = null;
    }

    return {
        register: dotnetRef => {
            unregister();
            ref = dotnetRef;
            document.addEventListener('keydown', onKeyDown);
            document.addEventListener('keyup', onKeyUp);
            document.addEventListener('click', onClick);
        },
        unregister,
    };
})();
