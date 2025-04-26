import styles from './search.module.scss';
import search from '../../assets/svg/search.svg';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchRecipes,
    selectFilter,
    updateFilter,
} from 'store/main-page.slice';

export function Search() {
    const filter = useSelector(selectFilter);
    const dispatch = useDispatch();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        dispatch(updateFilter({ SearchByTitle: e.target.value }));
    };

    return (
        <div className={styles.mainContainer}>
            <img src={search} alt='search' className={styles.icon} />

            <input
                type='text'
                placeholder='Что хотите найти?'
                className={styles.input}
                value={filter.SearchByTitle}
                onChange={handleChange}
            />

            <button
                className={styles.button}
                onClick={() => dispatch(fetchRecipes())}
            >
                Найти
            </button>
        </div>
    );
}
