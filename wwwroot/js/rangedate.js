$('input[name="daterange"]').daterangepicker({
    opens: 'right',
    autoUpdateInput: false,
    locale: {
        format: 'DD/MMM/YYYY',
        cancelLabel: 'Clear'
    }
});
$('input[name="daterange"]').on('apply.daterangepicker', function (ev, picker) {
    $(this).val(picker.startDate.format('DD/MMM/YYYY') + ' - ' + picker.endDate.format('DD/MMM/YYYY'));
});

$('input[name="daterange"]').on('cancel.daterangepicker', function (ev, picker) {
    const today = moment();

    // Update picker
    picker.setStartDate(today);
    picker.setEndDate(today);

    // Update input
    $(this).val(today.format('DD/MMM/YYYY') + ' - ' + today.format('DD/MMM/YYYY'));
});

$('#open-daterange').on('click', function () {
    $('#bs-rangepicker-dropdown').focus();
});
