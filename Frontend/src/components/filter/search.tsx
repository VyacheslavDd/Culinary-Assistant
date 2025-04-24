import { useState } from 'react';
import styles from './search.module.scss';
import search from '../../assets/svg/search.svg';

export function Search() {
    const [query, setQuery] = useState('');

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(e.target.value);
    };

    return (
        <div className={styles.mainContainer}>
            {query === '' && <img src={search} alt='search' className={styles.icon} />}
            <input
                type='text'
                placeholder='Что хотите найти?'
                className={styles.input}
                value={query}
                onChange={handleChange}
            />
            {query !== '' && (
                <button className={styles.button}>Найти</button>
            )}
        </div>
    );
}
