window.focusHelper = {
    focusElement: function (elementId) {
        var element = document.getElementById(elementId);
        if (element) {
            element.focus();
        }
    },
    setCursorToEnd: function (elementId) {
        var element = document.getElementById(elementId);
        if (element) {
            element.focus();
            var val = element.value;
            element.value = '';
            element.value = val;
        }
    }
};