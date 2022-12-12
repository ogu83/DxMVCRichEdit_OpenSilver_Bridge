importScripts('./nspell.js');
var checkers;

function checkWord(word) {
    for (var i = 0; i < checkers.length; i++)
        if (checkers[i].correct(word))
            return true;
    return false;
}

function getSuggestions(word) {
    var suggestions = [];
    for (var i = 0; i < checkers.length; i++)
        suggestions = suggestions.concat(checkers[i].suggest(word));
    return suggestions;
}

onmessage = function (e) {
    if (!checkers) {
        checkers = [];
        NSpell.dictionaries.forEach(function (dic) {
            checkers.push(new NSpell.nspell(dic));
        });
    }
    switch (e.data.command) {
        case 'checkWord': {
            var isCorrect = checkWord(e.data.word);
            postMessage({
                id: e.data.id,
                isCorrect: isCorrect,
                suggestions: isCorrect ? undefined : getSuggestions(e.data.word),
            });
            break;
        }
        case 'addWord': {
            checkers[0].add(e.data.word);
            break;
        }
    }
};