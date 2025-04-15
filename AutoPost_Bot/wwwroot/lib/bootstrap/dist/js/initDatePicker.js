window.initDatePickers = (ids) => {
    if (!window.tempusDominus) return;

    ids.forEach(id => {
        const element = document.getElementById(id);
        if (element && !element.classList.contains('td-initialized')) {
            new tempusDominus.TempusDominus(element, {
                display: {
                    components: {
                        calendar: true,
                        clock: true
                    }
                }
            });
            element.classList.add('td-initialized');
        }
    });
};