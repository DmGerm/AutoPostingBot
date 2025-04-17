window.initDatePickers = (ids) => {
    if (!window.tempusDominus) return;

    ids.forEach(id => {
        const wrapper = document.getElementById(id);
        if (wrapper && !wrapper.classList.contains('td-initialized')) {
            new tempusDominus.TempusDominus(wrapper, {
                container: wrapper, 
                display: {
                    components: {
                        calendar: true,
                        clock: true
                    },
                    buttons: {
                        today: true,
                        clear: true,
                        close: true
                    }
                }
            });
            wrapper.classList.add('td-initialized');
        }
    });
};