/*!
 * Copyright (c) Microsoft Corporation.
 * All rights reserved. See LICENSE in the project root for license information.
 */

import * as React from 'react';
import { IBrandProductsViewProps } from './brand-products';
import { getProductPageUrlSync } from '@msdyn365-commerce-modules/retail-actions';

export default (props: IBrandProductsViewProps) => {
    if (!props.config.productIds) {
        return (<div>暂无数据</div>)
    }
    const {
        context
    } = props
    var products = [];
    for (let index = 0; index < props.data.products.length; index++) {
        const number = Number(props.data.products[index].result?.RecordId)
        const href = getProductPageUrlSync(props.data.products[index].result?.Name || '', number, context && context.actionContext, undefined)
        products.push(
            <li>
                <a href={href}>
                    <img src={props.data.products[index].result?.PrimaryImageUrl} alt="" />
                    <div>{props.data.products[index].result?.Name}</div>
                    <div>{props.WKEAProducts && props.WKEAProducts.length ? props.WKEAProducts[index].ItemId : '暂无替代商品'}</div>
                    {/* <span>发货日:当天</span> */}
                    {/* @ts-ignore --强推的作用*/}
                    {/* <span>发货日:{props.data.products[index].result?.ExtensionProperties[1].Value?.StringValue || 14}个工作日</span>*/}
                    {/* {props.availableQuantity ? <div className="infos-fh1"><label>发货日：当日起</label></div> : <div className="infos-fh2"><label>发货日：<span>{+props.data.product.result.ExtensionProperties[1].Value.StringValue || 14}</span>个工作日</label></div>} */}
                    <span>型号:{props.data.products[index].result?.ItemId}</span>
                    <span>价格:<span>{props.data.products[index].result?.Price.toFixed(2)}</span></span>
                </a>
            </li>
        )
    }
    return (
        <div className='brand-page-products centre'>
            <div className="brand-page-products-bg">
                <div className="brand-page-products-type">{props.config.showText}</div>
                <ul className='clearboth'>
                    {products}
                </ul>
            </div>
        </div>
    );
};
