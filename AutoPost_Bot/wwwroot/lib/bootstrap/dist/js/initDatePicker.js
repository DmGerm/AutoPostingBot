window.initDatePicker = (id) => {
    const el = document.getElementById(id);
    if (el) {
        new tempusDominus.TempusDominus(el, {
            display: {
                components: {
                    calendar: true,
                    date: true,
                    month: true,
                    year: true,
                    decades: true,
                    clock: true,
                    hours: true,
                    minutes: true,
                    seconds: false
                }
            },
            localization: {
                locale: 'ru'
            }
        });
    }
}