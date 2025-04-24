import styles from './content.module.scss';
import { FilterList } from './filter-list';

const DIFFICULTY = ['Легко', 'Средне', 'Сложно'];
const CATEGORY = [
    'Завтраки',
    'Обеды',
    'Ужины',
    'Десерты',
    'Напитки',
    'Салаты',
    'Горячее',
];

const TAG = ['Веганские', 'Постные', 'Без глютена', 'Без лактозы'];

export function FilterContent() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.time}>
                <p className={styles.title}>Время приготовления</p>
                <p className={styles.filter}>
                    От <input type='text' className={styles.input_filter} /> мин
                    до <input type='text' className={styles.input_filter} /> мин
                </p>
            </div>
            <FilterList name='Сложность' list={DIFFICULTY} />
            <FilterList name='Категория' list={CATEGORY} />
            <FilterList name='Другое' list={TAG} />
        </div>
    );
}
