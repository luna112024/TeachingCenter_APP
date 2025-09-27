$(function () {
    $('.searchable-select').each(function () {
        const $container = $(this);
        const $button = $container.find('button.form-select');
        const $dropdownMenu = $container.find('.dropdown-menu');
        const $searchInput = $container.find('.dropdown-search-input');
        const hiddenName = $container.data('name');
        let selectedValue = $container.data('selected-value') || '';
        const ajaxUrl = $container.data('url')?.trim() || null;

        let $hiddenInput = $container.find(`input[type="hidden"][name="${hiddenName}"]`);
        if (!$hiddenInput.length) {
            $hiddenInput = $('<input type="hidden">').attr('name', hiddenName);
            $container.append($hiddenInput);
        }
        $hiddenInput.val(selectedValue);

        if (selectedValue) {
            const initialText = $button.find('.selected-account').text();
            if (initialText === "Select a Company" || initialText.trim() === "") {
                const $initialSelectedItem = $dropdownMenu.find(`.dropdown-item[data-value="${selectedValue}"]`);
                if ($initialSelectedItem.length) {
                    $button.find('.selected-account').text($initialSelectedItem.text()).removeClass('text-secondary');
                } else if (ajaxUrl) {
                    $button.find('.selected-account').text('Loading...').addClass('text-secondary');
                }
            }
        }

        function renderOptions(items) {
            $dropdownMenu.find('li').not(':first-child').remove();

            if (!Array.isArray(items)) {
                return $dropdownMenu.append('<li><a href="#" class="dropdown-item text-danger small">Invalid data format</a></li>');
            }

            if (items.length === 0) {
                return $dropdownMenu.append('<li><a href="#" class="dropdown-item text-muted small">No results found</a></li>');
            }

            items.forEach(item => {
                const isActive = (selectedValue.toString() === item.id.toString()) ? ' active' : '';
                const $li = $(`
                    <li>
                        <a href="#" class="dropdown-item${isActive}" data-value="${item.id}" title="${item.text}">
                            ${item.text}
                        </a>
                    </li>
                `);
                $dropdownMenu.append($li);
                if (isActive) {
                    $button.find('.selected-account').text(item.text).removeClass('text-secondary');
                }
            });
        }

        function fetchOptions(query = '') {
            if (!ajaxUrl) return;

            $dropdownMenu.find('li').not(':first-child').remove();
            $dropdownMenu.append('<li><span class="dropdown-item text-muted small"><i class="bx bx-loader bx-spin"></i> Loading...</span></li>');

            $.ajax({
                url: ajaxUrl,
                method: 'GET',
                data: { q: query },
                dataType: 'json'
            })
                .done(function (data) {
                    renderOptions(data);
                })
                .fail(function () {
                    $dropdownMenu.find('li').not(':first-child').remove();
                    $dropdownMenu.append('<li><a href="#" class="dropdown-item text-danger small">Error loading data. Check console.</a></li>');
                });
        }

        $searchInput.on('input', function () {
            const query = $(this).val().toLowerCase();
            if (ajaxUrl) {
                fetchOptions(query);
            } else {
                $dropdownMenu.find('li').not(':first-child').each(function () {
                    const text = $(this).find('.dropdown-item').text().toLowerCase();
                    $(this).toggle(text.includes(query));
                });
            }
        });

        $container.on('show.bs.dropdown', function () {
            $searchInput.val('').focus();
            if (ajaxUrl) {
                fetchOptions('');
            } else {
                $dropdownMenu.find('li').show();
            }
        });

        $dropdownMenu.on('click', '.dropdown-item', function (e) {
            e.preventDefault();
            const $item = $(this);
            const newValue = $item.data('value');
            const newText = $item.text();

            selectedValue = newValue;
            $hiddenInput.val(newValue);
            $container.data('selected-value', newValue);
            $button.find('.selected-account').text(newText).removeClass('text-secondary');

            $dropdownMenu.find('.dropdown-item').removeClass('active');
            $item.addClass('active');

            const bsDropdown = bootstrap.Dropdown.getInstance($button[0]);
            if (bsDropdown) {
                bsDropdown.hide();
            }

            $container.trigger('itemSelected', { id: newValue, text: newText });
        });

        if (ajaxUrl) {
            fetchOptions('');
        } else {
            if (selectedValue) {
                const $initialItem = $dropdownMenu.find(`.dropdown-item[data-value="${selectedValue}"]`);
                if ($initialItem.length) {
                    $button.find('.selected-account').text($initialItem.text()).removeClass('text-secondary');
                    $initialItem.addClass('active');
                }
            }
        }
    });
});
