window.studyhubFile = {
    download: (fileName, text) => {
        const url = URL.createObjectURL(new Blob([text], { type: 'application/json' }));
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.click();
        a.remove();
        URL.revokeObjectURL(url);
    },
};

window.studyhubStorage = {
    isPersistent: async () => !!navigator.storage?.persisted && await navigator.storage.persisted(),
    request: async () => !!navigator.storage?.persist && await navigator.storage.persist(),
};

window.studyhubSettings = {
    get: key => localStorage.getItem(key),
    set: (key, value) => localStorage.setItem(key, value),
    setLanguage: language => { document.documentElement.lang = language; },
};
