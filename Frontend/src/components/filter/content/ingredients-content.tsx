import { useState } from 'react';
import styles from './content.module.scss';

const INGREDIENTS = [
    'Ананас',
    'Артишок',
    'Айва',
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
    'Уксус',
    'Финик',
    'Хурма',
    'Цуккини',
    'Черника',
    'Шпинат',
    'Яблоко',
];

export function IngredientsContent() {
    const [query, setQuery] = useState('');

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
                        <li key={index} className={styles.li}>
                            {ingredient}
                        </li>
                    ))
                ) : (
                    <li>Ничего не найдено</li>
                )}
            </ul>
            <div className={styles.selectContainer}>
                <input type='checkbox' className={styles.checkbox} />
                <p>Искать только с этими продуктами</p>
            </div>
        </div>
    );
}
