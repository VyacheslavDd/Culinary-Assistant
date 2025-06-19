import styles from './filter-list.module.scss';
import { Category, CookingDifficulty, Tag } from '../../../../types';

type FilterListProps<T extends CookingDifficulty | Category | Tag> = {
    name: string;
    list: { name: string; value: T }[];
    selected?: T[];
    onChange: (selected: T[]) => void;
};

export function FilterList<T extends CookingDifficulty | Category | Tag>(
    props: FilterListProps<T>
) {
    const { name, list, selected = [], onChange } = props;

    const handleToggle = (value: T) => {
        const newSelected = selected.includes(value)
            ? selected.filter((item) => item !== value)
            : [...selected, value];

        onChange(newSelected);
    };

    return (
        <div className={styles.mainContainer}>
            <p className={styles.title}>{name}</p>
            <ul className={styles.list}>
                {list.map((item) => (
                    <li key={String(item.value)} className={styles.item}>
                        <input
                            type='checkbox'
                            className={styles.checkbox}
                            checked={selected.includes(item.value)}
                            onChange={() => handleToggle(item.value)}
                            id={`filter-${String(item.value)}`}
                        />
                        <label htmlFor={`filter-${String(item.value)}`}>
                            {item.name}
                        </label>
                    </li>
                ))}
            </ul>
        </div>
    );
}
