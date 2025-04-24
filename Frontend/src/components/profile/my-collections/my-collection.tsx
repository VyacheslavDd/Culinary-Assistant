import styles from './my-collection.module.scss';
import { ReactComponent as PlusIcon } from '../../../assets/svg/button_plus.svg';
import { NavLink } from 'react-router';
import { CardCollMin } from '../card-min/card-coll-min';
import { CardNew } from '../card-new/card-new';

export function MyCollection() {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.header}>
                <div className={styles.title}>
                    <h3 className={styles.h3}>Мои подборки</h3>
                    <button className={styles.button}>
                        <PlusIcon />
                    </button>
                </div>
                <NavLink to='/' className={styles.link}>Смотреть все</NavLink>
            </div>
            <ul className={styles.list}>
                <li className={styles.item}>
                    <CardCollMin theme='dark' />
                </li>
                <li className={styles.item}>
                    <CardCollMin theme='dark' />
                </li>
                <li className={styles.item}>
                    <CardCollMin theme='dark' />
                </li>
          
                <li className={styles.item}>
                    <CardNew />
                </li>
            </ul>
        </div>
    );
}
