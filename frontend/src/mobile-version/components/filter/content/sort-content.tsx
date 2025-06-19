import styles from './content.module.scss';
import { ReactComponent as SortUpIcon } from '../../../../assets/svg/sort_up.svg';
import { ReactComponent as SortDownIcon } from '../../../../assets/svg/sort_down.svg';
import { SortDirection, SortField } from '../index';

type props<T extends SortField> = {
    selectedField: T;
    selectedDirection: SortDirection;
    onChange: (field: T, direction: SortDirection) => void;
    isCollection?: boolean;
};

export function SortContent<T extends SortField>(props: props<T>) {
    const { selectedField, selectedDirection, onChange, isCollection } = props;
    const sortOptions = isCollection
        ? [
              { id: 'byPopularity', label: 'По популярности' },
              { id: 'byDate', label: 'По дате' },
              { id: 'byRating', label: 'По рейтингу' },
          ]
        : [
              { id: 'byPopularity', label: 'По популярности' },
              { id: 'byCookingTime', label: 'По времени' },
              { id: 'byCalories', label: 'По калорийности' },
          ];
    return (
        <div className={styles.mainContainer}>
            <ul className={styles.sort_list}>
                {sortOptions.map(({ id, label }) => (
                    <div
                        key={id}
                        className={`${styles.sort_item} ${
                            selectedField === id ? styles.active : ''
                        }`}
                    >
                        <input
                            type='radio'
                            id={`sort-${id}`}
                            name='sort'
                            className={styles.radio}
                            checked={selectedField === id}
                            onChange={() =>
                                onChange(id as T, selectedDirection)
                            }
                        />
                        <label htmlFor={`sort-${id}`}>{label}</label>
                    </div>
                ))}
            </ul>
            <div className={styles.sort_buttons}>
                <button
                    className={`${styles.sort_button} ${
                        selectedDirection === 'asc' ? styles.active : ''
                    }`}
                    onClick={() => onChange(selectedField, 'asc')}
                >
                    <SortUpIcon className={styles.icon} />
                    По возрастанию
                </button>
                <button
                    className={`${styles.sort_button} ${
                        selectedDirection === 'desc' ? styles.active : ''
                    }`}
                    onClick={() => onChange(selectedField, 'desc')}
                >
                    <SortDownIcon className={styles.icon} />
                    По убыванию
                </button>
            </div>
        </div>
    );
}
