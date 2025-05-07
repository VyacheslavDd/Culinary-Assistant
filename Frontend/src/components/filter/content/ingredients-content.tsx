import { useEffect, useState } from 'react';
import styles from './content.module.scss';
import { selectFilter, updateFilter } from 'store/main-page.slice';
import { useDispatch } from 'store/store';
import { useSelector } from 'react-redux';
import { selectIngredients } from 'store/ingredients.slice';

const INGREDIENTS = [
    'Ананас',
    'Артишок',
    'Айва',
    'Баклажан',
    'Банан',
    'Батат',
    'Груша',
    'Гранат',
    'Дыня',
    'Ежевика',
    'Инжир',
    'Киви',
    'Клубника',
    'Кокос',
    'Лимон',
    'Манго',
    'Малина',
    'Нектарин',
    'Огурец',
    'Персик',
    'Помело',
    'Ревень',
    'Слива',
    'Томат',
    'Тыква',
    'Уксус',
    'Финик',
    'Хурма',
    'Цуккини',
    'Черника',
    'Чеснок',
    'Шпинат',
    'Яблоко',
];

export function IngredientsContent() {
    const dispatch = useDispatch();
    const [query, setQuery] = useState('');
    const [selectedIngredients, setSelectedIngredients] = useState<string[]>(
        useSelector(selectFilter).SearchByIngredients
    );

    const toggleIngredient = (ingredient: string) => {
        setSelectedIngredients((prev) =>
            prev.includes(ingredient)
                ? prev.filter((item) => item !== ingredient)
                : [...prev, ingredient]
        );
    };

    useEffect(() => {
        dispatch(
            updateFilter({
                SearchByIngredients: selectedIngredients,
            })
        );
    }, [selectedIngredients, dispatch]);

    const filteredIngredients = INGREDIENTS.filter((ingredient) =>
        ingredient.toLowerCase().includes(query.toLowerCase())
    );

    return (
        <div className={styles.mainContainer}>
            <p className={styles.title}>Выберите имеющиеся ингредиенты</p>
            <input
                type='text'
                className={styles.input}
                placeholder='Или введите название...'
                value={query}
                onChange={(e) => setQuery(e.target.value)}
            />
            <ul className={styles.list}>
                {filteredIngredients.length > 0 ? (
                    filteredIngredients.map((ingredient, index) => (
                        <li
                            key={index}
                            className={`${styles.li} ${
                                selectedIngredients.includes(ingredient)
                                    ? styles.selected
                                    : ''
                            }`}
                            onClick={() => toggleIngredient(ingredient)}
                        >
                            {ingredient}
                        </li>
                    ))
                ) : (
                    <li>Ничего не найдено</li>
                )}
            </ul>
            <div className={styles.selectContainer}>
                <input
                    type='checkbox'
                    id='select'
                    className={styles.checkbox}
                    checked={useSelector(selectFilter).StrictIngredientsSearch}
                    onChange={(e) =>
                        dispatch(
                            updateFilter({
                                StrictIngredientsSearch: e.target.checked,
                            })
                        )
                    }
                />
                <label htmlFor='select'>Искать только с этими продуктами</label>
            </div>
        </div>
    );
}
