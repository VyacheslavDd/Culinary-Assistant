/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { NavLink } from 'react-router-dom';
import { useState } from 'react';
import styles from './my-collection.module.scss';
import { ReactComponent as PlusIcon } from '../../../../assets/svg/button_plus.svg';
import { CardCollMin } from '../card-min/card-coll-min';
import { CardNew } from '../card-new/card-new';
import { ShortCollection } from 'types/short-collection.type';
import { createCollectionApi } from 'store/api';
import { useDispatch, useSelector } from 'store/store';
import { fetchUsersCollections, selectUser } from 'store/user.slice';
import { NewCollectionOverlay } from './new/new-overlay';

type props = {
    items: ShortCollection[];
};

const MAX_ITEMS = 4;

export function MyCollection(props: props) {
    const { items } = props;
    const [showOverlay, setShowOverlay] = useState(false);
    const user = useSelector(selectUser);
    const dispatch = useDispatch();

    const displayItems = items.slice(0, MAX_ITEMS);
    const shouldShowCardNew = displayItems.length < MAX_ITEMS;

    const handleCreateCollection = async (name: string) => {
        try {
            await createCollectionApi({
                title: name,
                isPrivate: true,
                color: 'blue',
                userId: user!.id,
            });
            dispatch(fetchUsersCollections(user!.id));
        } catch (error) {
            console.error('Ошибка при создании подборки:', error);
            throw error;
        } finally {
            setShowOverlay(false);
        }
    };

    return (
        <div className={styles.mainContainer}>
            <div className={styles.header}>
                <div className={styles.title}>
                    <h3 className={styles.h3}>Мои подборки</h3>
                    <div className={styles.buttonContainer}>
                        <button
                            className={styles.button}
                            onClick={() => setShowOverlay((prev) => !prev)}
                        >
                            <PlusIcon className={styles.icon} />
                        </button>
                        {showOverlay && (
                            <NewCollectionOverlay
                                onClick={handleCreateCollection}
                            />
                        )}
                    </div>
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
