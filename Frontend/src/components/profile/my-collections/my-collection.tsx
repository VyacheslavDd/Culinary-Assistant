import styles from './my-collection.module.scss';
import { ReactComponent as PlusIcon } from '../../../assets/svg/button_plus.svg';
import { NavLink } from 'react-router';
import { CardCollMin } from '../card-min/card-coll-min';
import { CardNew } from '../card-new/card-new';
import { ShortCollection } from 'types/short-collection.type';

type props = {
    items: ShortCollection[];
};

const MAX_ITEMS = 4;

export function MyCollection(props: props) {
    const { items } = props;

    const displayItems = items.slice(0, MAX_ITEMS);
    const shouldShowCardNew = displayItems.length < MAX_ITEMS;

    return (
        <div className={styles.mainContainer}>
            <div className={styles.header}>
                <div className={styles.title}>
                    <h3 className={styles.h3}>Мои подборки</h3>
                    <button className={styles.button}>
                        <PlusIcon />
                    </button>
                </div>
                <NavLink to='/profile/my-collections' className={styles.link}>
                    Смотреть все
                </NavLink>
            </div>
            <ul className={styles.list}>
                {displayItems.map((collection) => (
                    <li className={styles.item} key={collection.id}>
                        <CardCollMin theme='dark' data={collection} />
                    </li>
                ))}

                {shouldShowCardNew && (
                    <li className={styles.item}>
                        <CardNew />
                    </li>
                )}
            </ul>
        </div>
    );
}
