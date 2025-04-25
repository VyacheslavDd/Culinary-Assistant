import styles from './content.module.scss';
import { ReactComponent as SortUpIcon } from '../../../assets/svg/sort_up.svg';
import { ReactComponent as SortDownIcon } from '../../../assets/svg/sort_down.svg';

export function SortContent() {
    return (
        <div className={styles.mainContainer}>
            <ul className={styles.sort_list}>
                <div className={`${styles.sort_item} ${styles.active}`}>
                    <input type='radio' name='sort' className={styles.radio} />
                    <label>По популярности</label>
                </div>
                <div className={`${styles.sort_item} ${styles.active}`}>
                    <input type='radio' name='sort' />
                    <label>По времени</label>
                </div>
                <div className={`${styles.sort_item} ${styles.active}`}>
                    <input type='radio' name='sort' />
                    <label>По калорийности</label>
                </div>
            </ul>
            <div className={styles.sort_buttons}>
                <button className={`${styles.sort_button} ${styles.active}`}>
                    <SortUpIcon className={styles.icon} />
                    По возрастанию
                </button>
                <button className={`${styles.sort_button}`}>
                    <SortDownIcon className={styles.icon} />
                    По убыванию
                </button>
            </div>
        </div>
    );
}
