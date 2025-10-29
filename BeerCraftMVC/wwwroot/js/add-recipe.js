ingredientsList.addEventListener('change', function (e) {
    if (e.target && e.target.classList.contains('ingredient-select')) {
        toggleNewIngredientFields(e.target);
    }
});
function toggleNewIngredientFields(selectElement) {
    const row = selectElement.closest('.ingredient-row');
    const newFieldsDiv = row.querySelector('.new-ingredient-fields');
    const nameInput = newFieldsDiv.querySelector('input[name*="NewIngredientName"]');
    const typeSelect = newFieldsDiv.querySelector('select[name*="NewIngredientTypeId"]');

    if (selectElement.value === '-1') {
        newFieldsDiv.style.display = 'block'; 

    } else {
        newFieldsDiv.style.display = 'none'; 
        if (nameInput) nameInput.value = '';
        if (typeSelect) typeSelect.value = ''; 

    }
}
document.querySelectorAll('.ingredient-select').forEach(select => {
    toggleNewIngredientFields(select);
});

function updateAttributes(row, index) {
    row.innerHTML = row.innerHTML.replace(/__INDEX__/g, index);
    const newSelect = row.querySelector('.ingredient-select');
    if (newSelect) {
        newSelect.addEventListener('change', function (e) {
            toggleNewIngredientFields(e.target);
        });
        toggleNewIngredientFields(newSelect);
    }
}