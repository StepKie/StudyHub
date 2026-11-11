// One JSON document per deck, keyed by deck id.
window.studyhubIdb = (() => {
    const DB_NAME = 'studyhub';
    const DECKS = 'decks';
    let dbPromise;

    function open() {
        return dbPromise ??= new Promise((resolve, reject) => {
            const req = indexedDB.open(DB_NAME, 1);
            req.onupgradeneeded = () => {
                const db = req.result;
                if (!db.objectStoreNames.contains(DECKS)) { db.createObjectStore(DECKS); }
            };
            req.onsuccess = () => resolve(req.result);
            req.onerror = () => reject(req.error);
        });
    }

    function read(op) {
        return open().then(db => new Promise((resolve, reject) => {
            const req = op(db.transaction(DECKS, 'readonly').objectStore(DECKS));
            req.onsuccess = () => resolve(req.result);
            req.onerror = () => reject(req.error);
        }));
    }

    function write(op) {
        return open().then(db => new Promise((resolve, reject) => {
            const tx = db.transaction(DECKS, 'readwrite');
            op(tx.objectStore(DECKS));
            tx.oncomplete = () => resolve();
            tx.onerror = () => reject(tx.error);
            tx.onabort = () => reject(tx.error);
        }));
    }

    return {
        getAll: () => read(s => s.getAll()),
        get: (id) => read(s => s.get(id)),
        put: (id, json) => write(s => { s.put(json, id); }),
        remove: (id) => write(s => { s.delete(id); }),
    };
})();
